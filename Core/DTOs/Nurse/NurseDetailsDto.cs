using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Nurse
{
    public class NurseDetailsDto
    {
        [Required]
        public required string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }
        [Display(Name = "Experience Years")]
        public int ExperienceYears { get; set; }
        [Display(Name = "Is Available?")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Is Verified?")]
        public bool IsVerified { get; set; }
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        public int VisitCount { get; set; }
    }
}
