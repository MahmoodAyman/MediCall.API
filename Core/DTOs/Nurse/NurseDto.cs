using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Nurse
{
    public class NurseDto
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }
        [Display(Name = "Experience Years")]
        public int ExperienceYears { get; set; }
    }
}
