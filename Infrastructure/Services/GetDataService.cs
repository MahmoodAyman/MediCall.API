using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Certificate;
using Core.DTOs.Illness;
using Core.DTOs.Service;
using Core.Interface;

namespace Infrastructure.Services
{
    public class GetDataService : IGetDataService
    {
        private readonly IGenericRepository<Core.Models.Certificate> _certificateRepository;
        private readonly IGenericRepository<Core.Models.Illness> _illnessRepository;
        private readonly IGenericRepository<Core.Models.Service> _serviceRepository;
        public GetDataService(IGenericRepository<Core.Models.Certificate> certificateRepository,
            IGenericRepository<Core.Models.Illness> illnessRepository,
            IGenericRepository<Core.Models.Service> serviceRepository)
        {
            _certificateRepository = certificateRepository;
            _illnessRepository = illnessRepository;
            _serviceRepository = serviceRepository;
        }
        public async Task<List<CertificateDetailsDto>> GetAllCertificatesAsync()
        {
            return (await _certificateRepository.GetAllAsync()).Select(c => new Core.DTOs.Certificate.CertificateDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();
        }
        public async Task<List<IllnessDetailsDto>> GetAllIllnessesAsync()
        {
            return (await _illnessRepository.GetAllAsync()).Select(i => new Core.DTOs.Illness.IllnessDetailsDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description
            }).ToList();
        }
        public async Task<List<ServiceDto>> GetAllServicesAsync()
        {
            return (await _serviceRepository.GetAllAsync()).Select(s => new Core.DTOs.Service.ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            }).ToList();
        }
    }
}
