using System;
using Core.DTOs;
using Core.Models;

namespace Core.Interface;

public interface INurseCertificateService
{
    Task<IEnumerable<NurseCertificate>> GetAllNurseCertificatesAsync(string nurseId);
    Task<NurseCertificate> GetNurseCertificateAsync(string nurseId, int certificateId);

    Task<NurseCertificate> AddCertificateAsync(string nurseId, NurseCertificateDTO certificateDto);

    Task<NurseCertificate> UpdateCertificateAsync(string nurseId, int certificateId, NurseCertificateDTO certificatedDto);

    Task<bool> DeleteCertificatedAsync(string nurseId, int certificateId);



}
