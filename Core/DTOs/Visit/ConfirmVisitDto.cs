using System;
using Core.Models;

namespace Core.DTOs.Visit;

public class ConfirmVisitDto
{
    public int Id { get; set; }

    public DateTime? ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }

    public Core.Enums.VisitStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public decimal ServiceCost => Services.Sum(s => s.BasePrice);
    public decimal TransportationCost { get; set; }

    public decimal CalculateTotalCost() => Services?.Sum(s => s.BasePrice) ?? 0 + TransportationCost;

    public required Location PatientLocation { get; set; }
    public required Location NurseLocation { get; set; }

    public required string NurseId { get; set; }
    public required string PatientId { get; set; }
    public virtual List<Core.Models.Service> Services { get; set; } = [];
    public virtual Core.Models.Nurse Nurse { get; set; } = null!;
    public virtual Core.Models.Patient Patient { get; set; } = null!;

    public virtual Core.Models.Payment Payment { get; set; } = null!;
    public virtual Reviewing Reviewing { get; set; } = null!;
}
