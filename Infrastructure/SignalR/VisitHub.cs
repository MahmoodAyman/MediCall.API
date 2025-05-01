using System;
using Core.DTOs.Visit;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class VisitHub : Hub
{
    public async Task SendNotificationToNurses(string nurseId, CreateVisitDto visit)
    {
        await Clients.User(nurseId).SendAsync("New Visit Request", nurseId, visit);
    }
    public async Task NotifyNurseAccepted(string paytientId, object visitDetails)
    {
        await Clients.User(paytientId).SendAsync("Nurse Accepted", visitDetails);
    }
}

public class NotificationHub : Hub
{
   
}
