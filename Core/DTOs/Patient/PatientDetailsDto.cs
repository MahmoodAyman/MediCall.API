using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Patient
{
    public class PatientDetailsDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Location { get; set; }
        public virtual List<PatientIllnesses> PatientIllnesses { get; set; }
        public virtual List<Visit> Visits { get; set; }
    
}
}
