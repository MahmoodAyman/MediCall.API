using System;

namespace Core.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Body { get; set; }
    public required Notification Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

}
