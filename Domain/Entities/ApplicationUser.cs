﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities {

    public class ApplicationUser : IdentityUser {

        public bool IsActive { get; set; } = true;
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
