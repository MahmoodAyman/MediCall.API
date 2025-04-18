using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Reviewing
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; } // 1 to 5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Visit Visit { get; set; } = null!;

    }
}
