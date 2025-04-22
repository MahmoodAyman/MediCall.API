using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NurseCertificate : BaseEntity
    {
        public required string NurseId { get; set; }
        public int CertificateId { get; set; }

        public required string FilePath { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
        public bool IsVerified { get; set; }

        public virtual Nurse Nurse { get; set; } = null!;
        public virtual Certificate Certificate { get; set; } = null!;
    }
}
