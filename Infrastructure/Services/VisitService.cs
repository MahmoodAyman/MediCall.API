using System;
using Core.DTOs.Visit;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class VisitService(MediCallContext _context) : IVisitService
{
    public Task<IReadOnlyList<Nurse>> GetNearNurses(CreateVisitDto visit)
    {
        throw new Exception("Stop");
    }
}
