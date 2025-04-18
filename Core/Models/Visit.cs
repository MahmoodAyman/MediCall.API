using System;
using Core.Enums;

namespace Core.Models;

public class Visit : BaseEntity
{
    public DateTime ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public VisitStatus Status { get; set; }

    // Is This List<decimal> or List<Service> ?? 
    public List<decimal> ServiceCost { get; set; } = [];
    public decimal TransportationCost { get; set; }

    public decimal Cost { get; set; }

    // TODO: Nurse and Patient Objects! 

}
