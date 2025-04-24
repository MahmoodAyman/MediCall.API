using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Patient
{
    public class CreatedPatientDto
    {
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }
        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public virtual List<PatientIllnesses> PatientIllnesses { get; set; } = [];
    }
}
