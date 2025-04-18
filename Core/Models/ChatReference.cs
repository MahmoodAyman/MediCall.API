using System;

namespace Core.Models;

public class ChatReference
{
    public int Id { get; set; }
    public required string FirebaseChatId { get; set; }

    public int PatientId { get; set; }
    public int NurseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
