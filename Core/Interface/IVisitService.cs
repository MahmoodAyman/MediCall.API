using System;
using Core.DTOs.Nurse;
using Core.DTOs.Visit;
using Core.Models;

namespace Core.Interface;

public interface IVisitService
{
    public Task<IReadOnlyList<NurseDetailsDto>> GetNearNurses(CreateVisitDto visit);
    public Task<string> CreatePendingVisitAsync(CreateVisitDto visitDto);

    public Task<(bool Success, string Message, Visit visit, NurseDetailsDto NurseDetails)> AcceptVisitByNurse(int visitId, string nurseId);

}
