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
        public decimal Amount => Visit.CalculateTotalCost();

        public decimal PlatformCommission => Visit.ServiceCost * 0.10m;

        public decimal NurseTotalProfit => Amount - PlatformCommission;
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public required string TransactionReference { get; set; }
        public int VisitId { get; set; }
        public virtual Visit Visit { get; set; } = null!;

        public bool NursePaid { get; set; } = false;
        public DateTime? NursePaymentDate { get; set; }
        public string? NursePaymentReference { get; set; }
    }
}
