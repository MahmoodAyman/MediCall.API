using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class PatientIllnessesDTO
    {
        public string PatientId { get; set; } = null!;
        public int IllnessId { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string? Notes { get; set; }
    }
}
