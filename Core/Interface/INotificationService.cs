using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Notification;
using Core.Models;

namespace Core.Interface
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string title, string? body,object arg);
        Task SendNotificationToAllAsync(string message);
        Task<List<NotificationDTO>> GetNotificationsAsync(string userId);
        Task<bool> MakeNotificationReadAsync(int notificationId,string userId);
    }
}
