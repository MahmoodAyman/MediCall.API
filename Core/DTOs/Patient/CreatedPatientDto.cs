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
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public virtual List<PatientIllnesses> PatientIllnesses { get; set; } = [];
    }
}
