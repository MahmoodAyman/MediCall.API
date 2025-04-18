using System;
using Core.Enums;

namespace Core.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Body { get; set; }
    public required NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public required string UserId { get; set; }
    public virtual AppUser User { get; set; } = null!;

}
