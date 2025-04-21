using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PatientIllnesses : BaseEntity
    {
        public required string PatientId { get; set; }
        public int IllnessId { get; set; }

        public DateTime? DiagnosisDate { get; set; }
        public string? Notes { get; set; }

        public virtual Illness Illness { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
    }
}
