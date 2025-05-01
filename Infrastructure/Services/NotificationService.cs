using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Notification;
using Core.Interface;
using Core.Models;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IGenericRepository<Notification> _notificationRepository;

        public NotificationService(IHubContext<NotificationHub> hubContext, IGenericRepository<Notification> notificationRepository)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
        }

        public async Task SendNotificationAsync(string userId, string title, string? body, object arg)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Body = body
            }, arg);
            var notification = new Notification
            {
                Title = title,
                Body = body,
                Type = Core.Enums.NotificationType.Visit,
                IsRead = false,
                UserId = userId
            };
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveAllAsync();
        }

        public async Task SendNotificationToAllAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task<List<NotificationDTO>> GetNotificationsAsync(string userId)
        {
            return [..(await _notificationRepository.FindAsync(n => n.UserId == userId)).ToList()
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })];
        }

        public async Task<bool> MakeNotificationReadAsync(int notificationId, string userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return false;
            }
            if (notification.UserId != userId)
            {
                return false;
            }
            notification.IsRead = true;
            _notificationRepository.Update(notification);
            await _notificationRepository.SaveAllAsync();
            return true;
        }
    }
}
