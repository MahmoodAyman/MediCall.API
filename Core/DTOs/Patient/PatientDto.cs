using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Patient
{
    public class PatientDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
