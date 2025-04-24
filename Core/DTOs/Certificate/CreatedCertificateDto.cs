using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Certificate
{
    public class CreatedCertificateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
