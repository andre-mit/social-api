using Social.Api.Models;
using Social.Api.ViewModels.AuthViewModels;
using Social.Api.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Api.Services.Interfaces
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
}
