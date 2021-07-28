using Social.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Api.ViewModels.UserViewModels
{
    public class ListUserResponseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public ICollection<Role> Roles { get; set; }

        public static implicit operator ListUserResponseViewModel(User user)
        {
            return new ListUserResponseViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Roles = user.Roles,
            };
        }
    }
}
