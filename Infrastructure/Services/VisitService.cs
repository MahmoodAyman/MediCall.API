    using System;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs.Nurse;
using Core.DTOs.Patient;
using Core.DTOs.Service;
using Core.DTOs.Visit;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class VisitService(IGenericRepository<Visit> visitRepository,
                            IGenericRepository<Service> serviceRepository,
                            IGenericRepository<Nurse> nurseRepository,
                            IGenericRepository<Patient> patientRepository,
                            INotificationService notificationService
                            ) : IVisitService
{
    //*private readonly MediCallContext _context = context;
    private readonly IGenericRepository<Visit> _visitRepository = visitRepository;
    private readonly IGenericRepository<Patient> _patientRepository = patientRepository;
    private readonly IGenericRepository<Nurse> _nurseRepository = nurseRepository;
    private readonly IGenericRepository<Service> _serviceRepository = serviceRepository;
    private readonly INotificationService _notificationService = notificationService;


    public async Task<int> CreatePendingVisitAsync(RequestNearNursesDTO visitDto)
    {
        var visit = new Visit
        {
            PatientId = visitDto.PatientId,
            PatientLocation = visitDto.PatientLocation,
            ScheduledDate = DateTime.UtcNow,
            Status = Core.Enums.VisitStatus.Pending,
            Services =[..await _serviceRepository.FindAsync(s => visitDto.ServicesIds.Contains(s.Id))],
        };
        await _visitRepository.AddAsync(visit);
        await _visitRepository.SaveAllAsync();

        return visit.Id;
    }

    public async Task<ResponseNearNursesDTO> GetNearNurses(RequestNearNursesDTO requestNeerNurses)
    {
        var responseNeerNurses = new ResponseNearNursesDTO();
        var errors = await Validation(requestNeerNurses);
        if(errors.Count > 0)
        {
            responseNeerNurses.Success = false;
            responseNeerNurses.Message = string.Join(" , ",errors);
            return responseNeerNurses;
        }

        var visitId = await CreatePendingVisitAsync(requestNeerNurses);

        var nurses =(await _nurseRepository.FindAsync(n => n.IsAvailable && n.IsVerified)).ToList();
        var nursesOrderedByLocation = nurses.Select(n => new
        {
            Nurse = n,
            Distance = CalculateDistance(requestNeerNurses.PatientLocation, n.Location)
        }).Where(x => x.Distance < 10).OrderBy(x => x.Distance).Select(x => x.Nurse).ToList();

        var visitServiceIds = requestNeerNurses.ServicesIds;

        nursesOrderedByLocation = [.. nursesOrderedByLocation.Where(n => visitServiceIds.All(visitServiceId => n.Services.Any(ns => ns.Id == visitServiceId)))];

        var matchedNurses = nursesOrderedByLocation.Select(n => new NurseDetailsDto
        {
            Id = n.Id,
            FirstName = n.FirstName,
            LastName = n.LastName,
            ExperienceYears = n.ExperienceYears,
            VisitCount = n.VisitCount
        }).ToList();

        var visit = await _visitRepository.GetByIdAsync(visitId)??throw new Exception("visit is null");
        responseNeerNurses.Nurses = matchedNurses;
        responseNeerNurses.Vist = ToVisitDto(visit);
        responseNeerNurses.Success = true;

        foreach( var nurse in matchedNurses)
        {

            await _notificationService.SendNotificationAsync(nurse.Id, "There is a visit near you", "A patient needs a service you are qualified to provide.", visit);
        }

        return responseNeerNurses;
    }


    



    private async Task<List<string>> Validation(RequestNearNursesDTO requestNeerNurses)
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(requestNeerNurses.PatientId))
        {
            errors.Add("Patient Id not provided");
        }
        else
        {
            if (await _patientRepository.GetByIdAsync(requestNeerNurses.PatientId) is null)
            {
                errors.Add("Patient not found");
            }
        }
        if (requestNeerNurses.PatientLocation == null)
        {
            errors.Add("Patient Location not provided");
        }
        else
        {
            if (requestNeerNurses.PatientLocation.Lat is < (-90) or > 90)
            {
                errors.Add("Patient Location Lat not valid");
            }
            if (requestNeerNurses.PatientLocation.Lng is < (-180) or > 180)
            {
                errors.Add("Patient Location Lng not valid");
            }
        }

        if (requestNeerNurses.ServicesIds == null || requestNeerNurses.ServicesIds.Count == 0)
        {
            errors.Add("Services Ids not provided");
        }
        else
        {
            foreach (var serviceId in requestNeerNurses.ServicesIds)
            {
                if (await _serviceRepository.GetByIdAsync(serviceId) is null)
                {
                    errors.Add($"Service with Id {serviceId} not found");
                }
            }
        }
        return errors;
    }

    private static double CalculateDistance(Location l1, Location l2)
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

    private static double DegreesToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }


    public static VisitDTO ToVisitDto(Visit visit)
    {
        return new VisitDTO
        {
            Id = visit.Id,
            ActualVisitDate = visit.ActualVisitDate,
            ScheduledDate = visit.ScheduledDate,
            Status = visit.Status,
            Notes = visit.Notes,
            CancellationReason = visit.CancellationReason,
            TransportationCost = visit.TransportationCost,
            ServiceCost = visit.ServiceCost,
            PatientLocation = visit.PatientLocation,
            NurseLocation = visit.NurseLocation,
            NurseId = visit.NurseId,
            PatientId = visit.PatientId,
            Services = [.. visit.Services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                BasePrice = s.BasePrice
            })],
            Nurse = visit.Nurse is not null
                ? new NurseDetailsDto
                {
                    Id = visit.Nurse.Id,
                    FirstName = visit.Nurse.FirstName,
                    LastName = visit.Nurse.LastName,
                    PhoneNumber = visit.Nurse.PhoneNumber,
                    ProfilePicture = visit.Nurse.ProfilePicture,
                    ExperienceYears = visit.Nurse.ExperienceYears,
                    VisitCount = visit.Nurse.VisitCount,
                    
                }
                : null,
            Patient = new PatientDetailsDto
            {
                Id = visit.Patient.Id,
                FirstName = visit.Patient.FirstName,
                LastName = visit.Patient.LastName,
                PhoneNumber = visit.Patient.PhoneNumber,
                DateOfBirth = DateOnly.Parse(visit.Patient.DateOfBirth.ToString()),
                ProfilePicture = visit.Patient.ProfilePicture,
                PatientIllnesses = visit.Patient.PatientIllnesses,
            }
        };
    }

    public async Task<ResponseNearNursesDTO> AcceptVisitByNurse(int visitId, string nurseId)
    {
        var response = new ResponseNearNursesDTO();
        var visit = await _visitRepository.GetByIdAsync(visitId);
        var nurse = await _nurseRepository.GetByIdAsync(nurseId);
        var errors = validAcceptVisitByNurse(visit, nurse);
        if (errors != null && errors.Count > 0)
        {
            response.Success = false;
            response.Message = string.Join(" , ", errors);
            return response;
        }
        await _notificationService.SendNotificationAsync(visit.PatientId, "There is a nurse near you.", "A nurse near you to perform the service",nurse);
        response.Success = true;
        response.Message = "Wait for the patient to answer.";

        return response;
    }



    public async Task<ResponseNearNursesDTO> AcceptNurseByPatient(int visitId, string nurseId,string patientId)
    {
        var response = new ResponseNearNursesDTO();
        var visit = await _visitRepository.GetByIdAsync(visitId);
        
        var nurse = await _nurseRepository.GetByIdAsync(nurseId);
        var errors = validAcceptVisitByNurse(visit, nurse);
        if (errors != null && errors.Count > 0)
        {
            response.Success = false;
            response.Message = string.Join(" , ", errors);
            
            return response;
        }

        if(visit.PatientId != patientId)
        {
            response.Success = false;
            response.Message = "You are not allowed to accept this visit";
            return response;
        }

        visit.Status = VisitStatus.Accepted;
        visit.NurseId = nurseId;
        visit.NurseLocation = nurse.Location;
        visit.ActualVisitDate = DateTime.UtcNow;
        visit.TransportationCost = (decimal)CalculateDistance(visit.PatientLocation, nurse.Location)*3;

        _visitRepository.Update(visit);
        await _visitRepository.SaveAllAsync();


        nurse.IsAvailable = false;
        _nurseRepository.Update(nurse);
        await _nurseRepository.SaveAllAsync();


        await _notificationService.SendNotificationAsync(nurseId, "Visit accepted", "You have accepted a visit", visit);

        response.Success = true;
        response.Message = "Visit accepted successfully";
        response.Vist = ToVisitDto(visit);
        response.Nurses = new List<NurseDetailsDto>
        {
            new NurseDetailsDto
            {
                Id = nurse.Id,
                FirstName = nurse.FirstName,
                LastName = nurse.LastName,
                PhoneNumber = nurse.PhoneNumber,
                ProfilePicture = nurse.ProfilePicture,
                ExperienceYears = nurse.ExperienceYears,
                VisitCount = nurse.VisitCount
            }
        };

        return response;
    }
    private List<string> validAcceptVisitByNurse(Visit? visit, Nurse? nurse)
    {
        var errors = new List<string>();

        if (nurse is null)
        {
            errors.Add("Nurse not found");
        }
        else
        {
            if (!nurse.IsAvailable)
            {
                errors.Add("Nurse not available");
            }
            if (!nurse.IsVerified)
            {
                errors.Add("Nurse not verified");
            }
        }

        if (visit is null)
        {
            errors.Add("Visit not found");
        }
        else
        {
            if (visit.Status == VisitStatus.Accepted)
            {
                errors.Add("Visit already accepted");
            }
        }
        return errors;
    }

    public async Task<ResponseNearNursesDTO> CancelVisitByPatient(int visitId, string patientId, string canclationReson)
    {
        var response = new ResponseNearNursesDTO();
        var visit = await _visitRepository.GetByIdAsync(visitId);
        var patient = await _patientRepository.GetByIdAsync(patientId);
        var errors = ValidCancelVisit(visit, patient);
        if (errors != null && errors.Count > 0)
        {
            response.Success = false;
            response.Message = string.Join(" , ", errors);

            return response;
        }
        if (visit.PatientId != patientId)
        {
            response.Success = false;
            response.Message = "You are not allowed to cancel this visit";
            return response;
        }

        visit.Status = VisitStatus.Canceled;
        visit.CancellationReason = canclationReson;
        _visitRepository.Update(visit);
        await _visitRepository.SaveAllAsync();

        var nurse = await _nurseRepository.GetByIdAsync(visit.NurseId) ?? throw new Exception("Nurse not found");
        nurse.IsAvailable = false;
        _nurseRepository.Update(nurse);
        await _nurseRepository.SaveAllAsync();

        await _notificationService.SendNotificationAsync(visit.NurseId, "Visit canceled", "The patient has canceled the visit", visit);
        response.Success = true;
        response.Message = "Visit canceled successfully";
        return response;
    }

    public async Task<ResponseNearNursesDTO> CancelVisitByNurse(int visitId, string nurseId, string canclationReson)
    {
        var response = new ResponseNearNursesDTO();
        var visit = await _visitRepository.GetByIdAsync(visitId);
        var nurse = await _nurseRepository.GetByIdAsync(nurseId);
        var errors = ValidCancelVisit(visit, nurse);
        if (errors != null && errors.Count > 0)
        {
            response.Success = false;
            response.Message = string.Join(" , ", errors);

            return response;
        }

        if (visit.NurseId != nurseId)
        {
            response.Success = false;
            response.Message = "You are not allowed to Cancel this visit";
            return response;
        }

        visit.Status = VisitStatus.Canceled;
        visit.CancellationReason = canclationReson;
        _visitRepository.Update(visit);
        await _visitRepository.SaveAllAsync();

        nurse.IsAvailable = true;
        _nurseRepository.Update(nurse);
        await _nurseRepository.SaveAllAsync();

        await _notificationService.SendNotificationAsync(visit.PatientId, "Visit canceled", "The nurse has canceled the visit", visit);
        response.Success = true;
        response.Message = "Visit canceled successfully";
        return response;
    }
    private List<string> ValidCancelVisit(Visit? visit, AppUser? patient)
    {
        var errors = new List<string>();
        if (patient is null)
        {
            errors.Add("Patient not found");
        }
        if (visit is null)
        {
            errors.Add("Visit not found");
        }
        else
        {
            if (visit.Status == VisitStatus.Done)
            {
                errors.Add("Visit is Done you can not cancl it");
            }
            else if(visit.Status == VisitStatus.Canceled)
            {
                errors.Add("Visit is already cancelled you can not cancl it again");
            }
        }
        return errors;
    }

    public async Task<ResponseNearNursesDTO> CompleteVisitByPatient(int visitId, string patientId)
    {
        var response = new ResponseNearNursesDTO();
        var visit = await _visitRepository.GetByIdAsync(visitId);
        var patient = await _nurseRepository.GetByIdAsync(visitId);
        var errors = ValidCancelVisit(visit, patient);
        if (errors != null && errors.Count > 0)
        {
            response.Success = false;
            response.Message = string.Join(" , ", errors);

            return response;
        }

        if (visit.PatientId != patientId)
        {
            response.Success = false;
            response.Message = "You are not allowed to complete this visit";
            return response;
        }

        visit.Status = VisitStatus.Done;
        visit.ActualVisitDate = DateTime.UtcNow;
        _visitRepository.Update(visit);
        await _visitRepository.SaveAllAsync();

        var nurse = await _nurseRepository.GetByIdAsync(visit.NurseId) ?? throw new Exception("Nurse not found");

        nurse.VisitCount++;
        nurse.IsAvailable = true;

        _nurseRepository.Update(nurse);
        await _nurseRepository.SaveAllAsync();

        await _notificationService.SendNotificationAsync(visit.NurseId, "Visit completed", "The patient has completed the visit", visit);

        response.Success = true;
        response.Message = "Visit completed successfully";
        response.Vist = ToVisitDto(visit);
        response.Nurses = new List<NurseDetailsDto>
        {
            new NurseDetailsDto
            {
                Id = nurse.Id,
                FirstName = nurse.FirstName,
                LastName = nurse.LastName,
                PhoneNumber = nurse.PhoneNumber,
                ProfilePicture = nurse.ProfilePicture,
                ExperienceYears = nurse.ExperienceYears,
                VisitCount = nurse.VisitCount
            }
        };
        return response;
    }
}