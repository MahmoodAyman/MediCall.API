using System;
using Core.DTOs.Visit;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class VisitService : IVisitService
{
    private readonly MediCallContext _context;
    private readonly UserManager<AppUser> _userManager;

    public VisitService(MediCallContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<Visit> CreateVisitAsync(ConfirmVisitDto visitDto, string nurseId)
    {
        var visit = new Visit
        {
            PatientId = visitDto.PatientId,
            NurseId = nurseId,
            PatientLocation = visitDto.PatientLocation,
            NurseLocation = visitDto.NurseLocation,
            //  Continue assinging Data
        };
        await _context.Visits.AddAsync(visit);
        await _context.SaveChangesAsync();
        return visit;
    }


    public async Task<IReadOnlyList<Nurse>> GetNearNurses(CreateVisitDto visit)
    {
        var patient = await _userManager.Users.FirstOrDefaultAsync(p => p.Id == visit.PatientId);
        var Nurses = await _context.Nurses.Where(n => n.IsAvailable && n.IsVerified).Where(n => n.Location != null).ToListAsync();
        var matchedNurses = Nurses.Where(nurse => visit.Services.All(requestedServices => nurse.Services.Any(nurseService => nurseService.Id == requestedServices.Id)));

        var sortNurses = matchedNurses.Select(nurse => new
        {
            Nurse = nurse,
            Distance = CacluateDistance(visit.PatientLocation, nurse.Location)
        }).OrderBy(x => x.Distance).Select(x => x.Nurse).ToList();

        return sortNurses;
    }


    private double CacluateDistance(Location l1, Location l2)
    {
        if (l1 == null || l2 == null) return double.MaxValue;

        var lat1 = DegreesToRadians(l1.Lat);
        var lng1 = DegreesToRadians(l1.Lng);
        var lat2 = DegreesToRadians(l2.Lat);
        var lng2 = DegreesToRadians(l2.Lng);

        var dLat = lat2 - lat1;
        var dlng = lng2 - lng1;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dlng / 2) * Math.Sin(dlng / 2); ;


        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var Distance = 6371 * c;

        return Distance;
    }
    private double DegreesToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }
}