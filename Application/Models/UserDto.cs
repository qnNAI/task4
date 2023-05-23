using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models {

    public class UserDto {

        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
