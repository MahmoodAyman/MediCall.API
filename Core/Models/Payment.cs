using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.Models
{
    public class Payment : BaseEntity
    {
        public int Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount => Visit.TotalCost;
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; } 
        public required string TransactionReference { get; set; }
        public int VisitId { get; set; }
        public virtual Visit Visit { get; set; } = null!;
    }
}
