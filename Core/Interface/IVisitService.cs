using System;
using Core.DTOs.Visit;
using Core.Models;

namespace Core.Interface;

public interface IVisitService
{
    public Task<IReadOnlyList<Nurse>> GetNearNurses(CreateVisitDto visit);

}
