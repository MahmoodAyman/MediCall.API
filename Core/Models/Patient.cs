using System;

namespace Core.Models;

public class Patient : AppUser
{
    public virtual List<PatientIllnesses> PatientIllnesses { get; set; } = [];
    public virtual List<Visit> Visits { get; set; } = [];
}
