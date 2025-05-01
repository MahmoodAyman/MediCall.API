using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Nurse;
using Core.DTOs.Patient;
using Core.DTOs.Service;
using Core.Enums;
using Core.Models;

namespace Core.DTOs.Visit
{
    public class VisitDTO
    {
        public int Id { get; set; }

        public DateTime? ActualVisitDate { get; set; }
        public DateTime ScheduledDate { get; set; }
        public VisitStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }

        public decimal ServiceCost { get; set; }
        public decimal TransportationCost { get; set; }

        public decimal TotalCost => ServiceCost + TransportationCost;

        public required Location PatientLocation { get; set; }
        public Location? NurseLocation { get; set; }

        public string? NurseId { get; set; }
        public required string PatientId { get; set; }
        public virtual List<ServiceDto> Services { get; set; } = [];
        public virtual NurseDetailsDto? Nurse { get; set; }
        public virtual PatientDetailsDto Patient { get; set; } = null!;
    }
}
