using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string ProfilePicture { get; set; }

        public double? Lat { get; set; }
        public double? Lng { get; set; }

        public bool IsDeleted { get; set; }
    }
}
