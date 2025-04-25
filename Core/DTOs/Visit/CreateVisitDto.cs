using System;
using Core.Models;

namespace Core.DTOs.Visit;

public class CreateVisitDto
{
    public required Location PatientLocation { get; set; }

    public List<string> Services { get; set; } = [];
    public DateTime? ActualVisitDate { get; set; }
    public DateTime ScheduledDate { get; set; }
}
