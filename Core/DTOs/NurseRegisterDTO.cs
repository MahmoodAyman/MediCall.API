using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.DTOs
{
    public class NurseRegisterDTO : RegisterDTO
    {
        public required string LicenseNumber { get; set; }
        public int ExperienceYears { get; set; }
        public List<NurseCertificateDTO> NurseCertificates { get; set; } = [];
    }
}
