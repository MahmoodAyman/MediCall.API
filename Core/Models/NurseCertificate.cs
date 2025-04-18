using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NurseCertificate
    {
        public int NurseId { get; set; }
        public int CertificateId { get; set; }
        public required string FilePath { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public virtual Nurse? Nurse { get; set; }
        public virtual Certificate? Certificate { get; set; }
    }
}
