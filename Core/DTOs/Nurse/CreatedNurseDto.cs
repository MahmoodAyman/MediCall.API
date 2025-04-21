using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Nurse
{
    public class CreatedNurseDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int ExperienceYears { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerified { get; set; }
        public virtual List<NurseCertificate> Certificates { get; set; } = new();
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
