using System;
using Core.Models;

namespace Core.Interface;

public interface IServiceService
{
    public Task<IReadOnlyList<Service>> GetAllServicesAsync();

}
