using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Social.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [Required]
        [MaxLength(36)]
        public string UserName { get; set; }
        [Phone(ErrorMessage = "Invalid phone")]
        [MaxLength(40)]
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public ICollection<Role> Roles { get; set; }

        [JsonIgnore]
        public ICollection<AccessToken> AccessTokens { get; set; }
    }
}
