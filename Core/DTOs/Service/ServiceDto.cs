using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Service
{
    public class ServiceDto
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }
    }
}
