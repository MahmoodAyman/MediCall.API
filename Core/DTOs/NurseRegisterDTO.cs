using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class NurseRegisterDTO : RegisterDTO
    {
        public required string LicenseNumber { get; set; }
        public int ExperienceYears { get; set; }
    }
}
