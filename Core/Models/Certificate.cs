using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.Models
{
    public class Certificate : BaseEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsRequired { get; set; }
        public bool IsExpirable { get; set; }
        public string? Description { get; set; }

    }
}
