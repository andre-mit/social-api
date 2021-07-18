using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.ViewModels.AuthViewModels
{
    public class LoginRequestViewModel
    {
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
