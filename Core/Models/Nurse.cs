using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.Models
{
    public class Nurse : AppUser    
    {
        public required string LicenseNumber { get; set; }
        public int ExperienceYears { get; set; }
        public bool IsAvailable { get; set; }
        public int visitCount = 0;
        public bool IsVerified { get; set; } = false;
        public virtual List<Visit> Visits { get; set; } = [];
        public virtual List<NurseCertificate> Certificates { get; set; } = [];

    }
}
