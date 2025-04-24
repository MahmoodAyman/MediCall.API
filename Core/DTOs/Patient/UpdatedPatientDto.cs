using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Patient
{
    public class UpdatedPatientDto
    {
        public int Id { get; set; }
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
        public Location? Location { get; set; }
        public virtual List<PatientIllnesses> PatientIllnesses { get; set; }
    }
}
