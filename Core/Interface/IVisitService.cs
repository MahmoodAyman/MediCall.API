using System;
using Core.DTOs.Nurse;
using Core.DTOs.Visit;
using Core.Models;

namespace Core.Interface;

public interface IVisitService
{
    public Task<IReadOnlyList<NurseDetailsDto>> GetNearNurses(CreateVisitDto visit);
    public Task<Visit> CreateVisitAsync(ConfirmVisitDto visitDto, string nurseId);

}
