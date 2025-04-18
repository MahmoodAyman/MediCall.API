using System;
using Core.Enums;

namespace Core.Models;

public class Visit 
{
    public int Id { get; set; }
    public DateTime ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public VisitStatus Status { get; set; }

    // Is This List<decimal> or List<Service> ?? 
    public decimal ServiceCost => Services.Sum(s => s.BasePrice); // TODO: Check if this is correct!
    public decimal TransportationCost { get; set; }
    public decimal TotalCost => ServiceCost + TransportationCost; // TODO: Check if this is correct!
    public decimal Cost { get; set; }

    // TODO: Nurse and Patient Objects! 
    public virtual List<Service> Services { get; set; } = new();

}
