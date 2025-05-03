using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
} 