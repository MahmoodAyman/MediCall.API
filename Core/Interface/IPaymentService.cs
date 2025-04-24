using System;
using Core.Enums;
using Core.Models;

namespace Core.Interface;

public interface IPaymentService
{
    Task<Payment> CreateOrUpdatePayment(int id);
    Task<Payment> UpdatePaymentStatus(string transactionReference, PaymentStatus status);
    Task<bool> ProcessNursePayment(int id);
}
