using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class VerifieNurseCertificateController : Controller
{
    private readonly IGenericRepository<NurseCertificate> _nurseCertificateRepository;
    private readonly IGenericRepository<Nurse> _nurseRepository;
    private readonly IGenericRepository<Certificate> _certificateRepository;

    public VerifieNurseCertificateController(
        IGenericRepository<NurseCertificate> nurseCertificateRepository,
        IGenericRepository<Nurse> nurseRepository,
        IGenericRepository<Certificate> certificateRepository)
    {
        _nurseCertificateRepository = nurseCertificateRepository;
        _nurseRepository = nurseRepository;
        _certificateRepository = certificateRepository;
    }

    public async Task<IActionResult> Index()
    {
        var nurseCertificates = await _nurseCertificateRepository.FindAsync(nc => nc.IsVerified == false);
        return View(nurseCertificates);
    }

    public async Task<IActionResult> Verify(string NurseId, int CertificateId)
    {
        var nurseCertificate = await _nurseCertificateRepository.GetByIdAsync( NurseId, CertificateId );
        if (nurseCertificate == null)
        {
            return NotFound();
        }

        nurseCertificate.IsVerified = true;
        _nurseCertificateRepository.Update(nurseCertificate);
        await _nurseCertificateRepository.SaveAllAsync();

        await VerifyNurse(nurseCertificate.NurseId);

        return RedirectToAction(nameof(Index));
    }

    private async Task VerifyNurse(string id)
    {
        var nurse = await _nurseRepository.GetByIdAsync(id);
        if (nurse == null)
        {
            return;
        }

        var requiredCertificatesCount = (await _certificateRepository.FindAsync(c => c.IsRequired)).Count();
        var verifiedRequiredCertificatesCount = nurse.Certificates.Count(c => c.IsVerified && c.Certificate.IsRequired);

        nurse.IsVerified = verifiedRequiredCertificatesCount == requiredCertificatesCount;
        

        _nurseRepository.Update(nurse);
        await _nurseRepository.SaveAllAsync();
    }
}
