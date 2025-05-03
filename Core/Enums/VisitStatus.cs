using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums;

public enum VisitStatus
{
    Pending,
    Accepted,
    Confirmed,
    Done,
    PendingInquiry,
    PaymentPending,
    InProgress,
    Canceled,
    PendingPayment
}
