using System;
using Core.Enums;

namespace Core.Models;

public class Visit : BaseEntity
{
    public int Id { get; set; }

    public DateTime? ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }

    public VisitStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public decimal ServiceCost => Services.Sum(s => s.BasePrice); 
    public decimal TransportationCost { get; set; }
    public decimal TotalCost => ServiceCost + TransportationCost;

    public required Location PatientLocation { get; set; }
    public required Location NurseLocation { get; set; }

    public required string NurseId {  get; set; }
    public required string PatientId {  get; set; }
    public virtual List<Service> Services { get; set; } = [];
    public virtual Nurse Nurse { get; set; } =null!;
    public virtual Patient Patient { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
    public virtual Reviewing Reviewing { get; set; } = null!;
}
