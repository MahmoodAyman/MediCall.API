using System;
using Core.DTOs.Nurse;
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


    public async Task<IReadOnlyList<NurseDetailsDto>> GetNearNurses(CreateVisitDto visit)
    {
        var patient = await _context.Users.FirstOrDefaultAsync(p => p.Id == visit.PatientId);
        var nurses = _context.Nurses.Where(n => n.IsAvailable && n.IsVerified).ToList();
        // nurses = nurses.Where(n => visit.Services.All(requestedService => n.Services.Any(s => s.Id == requestedService.Id)));
        var nursesOrderedByLocation = nurses.Select(n => new
        {
            Nurse = n,
            Distance = CacluateDistance(visit.PatientLocation, n.Location)
        }).Where(x => x.Distance < 10).OrderBy(x => x.Distance).Select(x => x.Nurse).ToList();
        var visitServiceIds = visit.Services.Select(s => s.Id).ToList();

        nursesOrderedByLocation = nursesOrderedByLocation
            .Where(n => visitServiceIds.All(visitServiceId => n.Services.Any(ns => ns.Id == visitServiceId))).ToList();

        var matchedNurses = new List<NurseDetailsDto>();

        foreach (var nurse in nursesOrderedByLocation)
        {
            matchedNurses.Add(new NurseDetailsDto
            {
                FirstName = nurse.FirstName,
                LastName = nurse.LastName,
                Id = nurse.Id,
                ExperienceYears = nurse.ExperienceYears,
                VisitCount = nurse.VisitCount,
            });
        }
        return [.. matchedNurses];
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


    private bool CompareServices(List<Service> NurseProvidedServices, List<Service> VisitRequestedServices)
    {
        foreach (var service in VisitRequestedServices)
        {
            if (!NurseProvidedServices.Contains(service))
            {
                return false;
            }
        }
        return true;
    }
    private double DegreesToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }
}