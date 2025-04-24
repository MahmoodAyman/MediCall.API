using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Certificate
{
    public class UpdatedCertificateDto
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Display(Name = "Is Required?")]
        public bool IsRequired { get; set; }
        [Display(Name = "Is Expirable?")]
        public bool IsExpirable { get; set; }

        public string? Description { get; set; }
    }
}
