using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Certificate
{
    public class CertificateDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsExpirable { get; set; }
        public string? Description { get; set; }
    }
}
