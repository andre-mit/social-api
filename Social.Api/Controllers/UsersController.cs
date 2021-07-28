using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Api.Contexts;
using Social.Api.Models;
using Social.Api.Services;
using Social.Api.Services.Interfaces;
using Social.Api.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public readonly ApplicationDbContext _context;

        public UsersController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<ListUserResponseViewModel>> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors).ToList());

            try
            {
                var user = await _userService.CreateUserAsync(model);

                return Ok(user);
            }
            catch (DuplicateNameException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("follow/{id}")]
        [Authorize]
        public async Task<IActionResult> FollowUser(Guid id)
        {
            var followUser = await _context.Users.AsNoTracking().AnyAsync(x => x.Id == id);

            if (!followUser)
                return NotFound("User not found");

            var userId = Guid.Parse(User.Claims.ToList().FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var duplicate = await _context.Follows.AnyAsync(x => x.FollowerId == userId && x.FollowingId == id);
            if (duplicate)
                return Conflict("User already follow");

            var date = DateTime.UtcNow;
            await _context.Follows
                .AddAsync(new Follow
                {
                    FollowerId = userId,
                    FollowingId = id,
                    Date = date
                });

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("unfollow/{id}")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser(Guid id)
        {
            var userId = Guid.Parse(User.Claims.ToList().FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var follow = await _context.Follows.AsNoTracking().FirstOrDefaultAsync(x => x.FollowerId == userId && x.FollowingId == id);
            if (follow is null)
                return BadRequest("User not follow");

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
