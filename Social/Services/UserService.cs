﻿using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Social.Contexts;
using Social.Helpers;
using Social.Models;
using Social.ViewModels.AuthViewModels;
using Social.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Authenticate user and return yours information and jwt token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns>User information, jwt token and refresh token | null | InvalidOperationException if wrong password</returns>
        Task<LoginResponseViewModel> AuthenticateAsync(LoginRequestViewModel model, string ipAddress);
        Task<LoginResponseViewModel> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<ListUserResponseViewModel> CreateUserAsync(CreateUserViewModel model);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
    }

    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public UserService(
            ApplicationDbContext context,
            JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        public async Task<LoginResponseViewModel> AuthenticateAsync(LoginRequestViewModel model, string ipAddress)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                throw new InvalidOperationException("Wrong password");
            }

            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
            user.AccessTokens.Add(refreshToken);
            _context.Update(user);
            _context.SaveChanges();

            return new LoginResponseViewModel(user, jwtToken, refreshToken.Token);
        }

        public async Task<LoginResponseViewModel> RefreshTokenAsync(string token, string ipAddress)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccessTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.AccessTokens.First(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.AccessTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new LoginResponseViewModel(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<ListUserResponseViewModel> CreateUserAsync(CreateUserViewModel model)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == model.Email || x.UserName == model.UserName);
            if (exists)
                throw new DuplicateNameException("User already registered");

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
            };
            var response = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return response.Entity;
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.AccessTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.AccessTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(Guid id)
        {
            return _context.Users.Find(id);
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private AccessToken generateRefreshToken(string ipAddress)
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            return new AccessToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
