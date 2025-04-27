using System;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ServiceService(MediCallContext _context) : IServiceService
{
    public async Task<IReadOnlyList<Service>> GetAllServicesAsync()
    {
        var services = await _context.Services.ToListAsync();
        return services;
    }
}
