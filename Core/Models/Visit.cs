using System;
using Core.Enums;

namespace Core.Models;

public class Visit
{
    public int Id { get; set; }

    public DateTime? ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }

    public VisitStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public decimal ServiceCost => Services.Sum(s => s.BasePrice);
    public decimal TransportationCost { get; set; }

    public decimal CalculateTotalCost() => Services?.Sum(s => s.BasePrice) ?? 0 + TransportationCost;

    public required Location PatientLocation { get; set; }
    public required Location NurseLocation { get; set; }

    public string? NurseId { get; set; }
    public required string PatientId { get; set; }
    public virtual List<Service> Services { get; set; } = [];
    public virtual Nurse Nurse { get; set; } = null!;
    public virtual Patient Patient { get; set; } = null!;
}
