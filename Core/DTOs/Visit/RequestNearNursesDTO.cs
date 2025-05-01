using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.DTOs.Visit
{
    public class RequestNearNursesDTO
    {
        public required string PatientId { get; set; }
        public required Location PatientLocation { get; set; }
        public List<int> ServicesIds { get; set; } = [];
    }
}
