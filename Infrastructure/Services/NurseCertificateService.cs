using System;
using Core.DTOs;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class NurseCertificateService(IGenericRepository<NurseCertificate> _repository, IUploadFileService _uploadFileService) : INurseCertificateService
{
    public async Task<NurseCertificate> AddCertificateAsync(string nurseId, NurseCertificateDTO certificateDto)
    {

        if (!IsFileValid(certificateDto.File))
        {
            throw new Exception("Image is not valid. Allowed formats: jpg, jpeg, png. Max size: 2MB.");
        }

        var filePath = await _uploadFileService.UploadFile(certificateDto.File);
        var nurseCertifice = new NurseCertificate
        {
            NurseId = nurseId,
            CertificateId = certificateDto.CertificateId,
            FilePath = filePath,
            ExpirationDate = certificateDto.ExpirationDate,
            IsVerified = false,
        };
        await _repository.AddAsync(nurseCertifice);
        await _repository.SaveAllAsync();
        return nurseCertifice;
    }

    public async Task<bool> DeleteCertificatedAsync(string nurseId, int certificateId)
    {
        var certificate = await _repository.GetByIdAsync(nurseId, certificateId);
        if (certificate == null) return false;
        certificate.IsDeleted = true;
        _repository.Update(certificate);
        return await _repository.SaveAllAsync();
    }

    public async Task<IEnumerable<NurseCertificate>> GetAllNurseCertificatesAsync(string nurseId)
    {
        return await _repository.FindAsync(nc => nc.NurseId == nurseId);

    }

    public async Task<NurseCertificate> GetNurseCertificateAsync(string nurseId, int certificateId)
    {
        var certificate = await _repository.GetByIdAsync(nurseId, certificateId);
        if (certificate == null) return null!;
        return certificate;
    }

    public async Task<NurseCertificate> UpdateCertificateAsync(string nurseId, int certificateId, NurseCertificateDTO certificatedDto)
    {
        var certificate = await _repository.GetByIdAsync(nurseId, certificateId);
        if (certificate == null) return null!;
        if (certificatedDto.File != null)
        {
            if (!IsFileValid(certificatedDto.File))
            {
                throw new Exception("Image is not valid. Allowed formats: jpg, jpeg, png. Max size: 2MB.");
            }
            certificate.FilePath = await _uploadFileService.UploadFile(certificatedDto.File);
        }
        certificate.ExpirationDate = certificatedDto.ExpirationDate;
        certificate.IsVerified = false;
        _repository.Update(certificate);
        await _repository.SaveAllAsync();
        return certificate;
    }

    private static bool IsFileValid(IFormFile file)
    {
        if (file is null) return true;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(fileExtension))
        {
            return false;
        }
        if (file.Length > 2 * 1024 * 1024)
        {
            return false;
        }
        return true;
    }
}
