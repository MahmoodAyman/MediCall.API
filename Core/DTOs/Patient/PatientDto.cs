using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Patient
{
    public class PatientDto
    {
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }
    }
}
