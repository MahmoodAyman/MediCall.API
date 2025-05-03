using Core.DTOs.Payment;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreateOrUpdatePayment(int visitId);
    Task<Payment> UpdatePaymentStatus(string transactionReference, PaymentStatus status);
    Task<bool> ProcessNursePayment(int visitId);
    Task<bool> ProcessPaymentRefund(int visitId, string cancellationReason);
    Task<PaymentStatus> HandlePaymentWebhook(PaymentWebhookDto webhookData);
}
