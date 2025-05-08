using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Nurse;
using Core.DTOs.Patient;

namespace Core.DTOs.Visit
{
    public class ResponseNearNursesDTO
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public VisitDTO? Visit { get; set; }
        public List<NurseDetailsDto> Nurses { get; set; } = [];



    }
}
