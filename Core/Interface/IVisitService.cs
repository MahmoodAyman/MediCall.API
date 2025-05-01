using System;
using Core.DTOs.Nurse;
using Core.DTOs.Visit;
using Core.Models;

namespace Core.Interface;

public interface IVisitService
{
    Task<ResponseNearNursesDTO> GetNearNurses(RequestNearNursesDTO requestNeerNurses);
    Task<int> CreatePendingVisitAsync(RequestNearNursesDTO visitDto);

    Task<ResponseNearNursesDTO> AcceptVisitByNurse(int visitId, string nurseId);
    Task<ResponseNearNursesDTO> AcceptNurseByPatient(int visitId, string nurseId);

}
