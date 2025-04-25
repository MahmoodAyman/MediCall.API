using System;
using Core.Models;

namespace Core.DTOs.Visit;

public class CreateVisitDto
{
    public required Location PatientLocation { get; set; }

    public virtual List<Core.Models.Service> Services { get; set; } = [];
    public DateTime? ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public required string PatientId { get; set; }
}
