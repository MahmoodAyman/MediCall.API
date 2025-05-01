using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Certificate;
using Core.DTOs.Illness;
using Core.DTOs.Service;
using Core.Models;

namespace Core.Interface
{
    public interface IGetDataService
    {
        Task<List<CertificateDetailsDto>> GetAllCertificatesAsync();
        Task<List<IllnessDetailsDto>> GetAllIllnessesAsync();
        Task<List<ServiceDto>> GetAllServicesAsync();
    }
}
