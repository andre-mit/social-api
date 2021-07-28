using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Social.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [MaxLength(250)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        [Required]
        [MaxLength(36)]
        public string UserName { get; set; }
        [MaxLength(40)]
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public ICollection<Role> Roles { get; set; }

        public ICollection<AccessToken> AccessTokens { get; set; }
        public ICollection<Follow> Following { get; set; }
        public ICollection<Follow> Followers { get; set; }
    }
}
