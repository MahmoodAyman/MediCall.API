using System;

namespace Core.Models;

public class Patient : AppUser
{

    public virtual List<Illness> PatientIllnesses { get; set; } = new List<Illness>();
}
