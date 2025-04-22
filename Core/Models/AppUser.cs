using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interface;
using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class AppUser : IdentityUser , IDeleteable , IAuditable
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string? ProfilePicture { get; set; }

        public Location? Location { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual List<Notification> Notifications { get; set; } = [];
        public virtual List<ChatReference> ChatReferences { get; set; } = [];

    }
}
