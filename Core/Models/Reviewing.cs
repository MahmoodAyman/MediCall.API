using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Reviewing : BaseEntity
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // 1 to 5
        public virtual Visit Visit { get; set; } = null!;

    }
}
