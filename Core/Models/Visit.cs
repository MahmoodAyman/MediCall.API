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
    public decimal ServiceCost => Services.Sum(s => s.BasePrice); 
    public decimal TransportationCost { get; set; }
    public decimal TotalCost => ServiceCost + TransportationCost;
    public decimal Cost { get; set; }
    public string? NurseId {  get; set; }
    public required string PatioentId {  get; set; }

    public virtual List<Service> Services { get; set; } = new List<Service>();
    // TODO: Patient Objects! 
    public virtual Nurse? Nurse { get; set; }

}
