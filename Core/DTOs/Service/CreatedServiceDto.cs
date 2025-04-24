using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Service
{
    public class CreatedServiceDto
    {
        [Required]
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }
    }
}
