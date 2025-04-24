using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Certificate
{
    public class CertificateDto
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
