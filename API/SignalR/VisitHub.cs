using System;
using Core.DTOs.Visit;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class VisitHub : Hub
{
    public async Task SendNotificationToNurses(string nurseId, CreateVisitDto visit)
    {
        await Clients.All.SendAsync("New Visit Request", nurseId, visit);
    }
}
