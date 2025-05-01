using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Notification
{
    public class NotificationDTO

    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Message { get; set; }
        public required string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? Body { get; set; }
    }
}
