using System.Runtime.Serialization.Formatters;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class NurseCertificateDTO
    {
        public int CertificateId { get; set; }
        public required IFormFile File{ get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}