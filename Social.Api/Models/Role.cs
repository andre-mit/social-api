using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Api.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
