using System;

namespace Core.Models;

public class Illness
{
    public int Id { get; set; }
    public required string IllnessQuestionText { get; set; }
    public required bool IllnessPatientAnswer { get; set; }

    public string? PatientNotes { get; set; } = string.Empty;
}
