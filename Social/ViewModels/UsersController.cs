using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Services;
using Social.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Social.ViewModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
    }
}
