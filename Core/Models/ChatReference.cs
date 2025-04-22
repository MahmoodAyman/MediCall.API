using System;

namespace Core.Models;

public class ChatReference : BaseEntity
{
    public int Id { get; set; }
    public required string FirebaseChatId { get; set; }

    public required string PatientId { get; set; }
    public required string NurseId { get; set; }
    public bool IsActive { get; set; }

    public virtual Patient Patient { get; set; } = null!;
    public virtual Nurse Nurse { get; set; } = null!;

}
