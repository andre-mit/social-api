using Social.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Social.ViewModels.AuthViewModels
{
    public class LoginResponseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        public LoginResponseViewModel(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Name = user.Name;
            UserName = user.UserName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
