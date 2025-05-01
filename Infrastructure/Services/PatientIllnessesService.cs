using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Interface;
using Core.Models;

namespace Infrastructure.Services
{
    public class PatientIllnessesService(
        IGenericRepository<PatientIllnesses> patientIllnessesRepository
        ) : IPatientIllnessesService
    {
        private readonly IGenericRepository<PatientIllnesses> _patientIllnessesRepository = patientIllnessesRepository;
        public async Task<PatientIllnessesDTO> AddPatientIllnessAsync(PatientIllnessesDTO patientIllness)
        {
            if (!await Validation(patientIllness))
            {
                throw new Exception("Invalid patient or illness ID");
            }
            var patientIllnesses = new PatientIllnesses
            {
                PatientId = patientIllness.PatientId,
                IllnessId = patientIllness.IllnessId,
                DiagnosisDate = patientIllness.DiagnosisDate,
                Notes = patientIllness.Notes
            };
            await _patientIllnessesRepository.AddAsync(patientIllnesses);
            await _patientIllnessesRepository.SaveAllAsync();
            return patientIllness;
        }

        public async Task DeletePatientIllnessAsync(int id,string userId)
        {
            var patientIllness = await _patientIllnessesRepository.GetByIdAsync(id, userId);
            if (patientIllness == null)
            {
                throw new Exception("Patient illness not found");
            }
            patientIllness.IsDeleted= true;
            _patientIllnessesRepository.Update(patientIllness);
            await _patientIllnessesRepository.SaveAllAsync();

        }


        public async Task<PatientIllnessesDTO> UpdatePatientIllnessAsync(PatientIllnessesDTO patientIllness)
        {
            if(!await Validation(patientIllness))
            {
                throw new Exception("Invalid patient or illness ID");
            }
            var existingPatientIllness = await _patientIllnessesRepository.GetByIdAsync(patientIllness.IllnessId,patientIllness.PatientId);

            if (existingPatientIllness == null)
            {
                throw new Exception("Patient illness not found");
            }

            existingPatientIllness.Notes = patientIllness.Notes;
            existingPatientIllness.DiagnosisDate = patientIllness.DiagnosisDate;

            _patientIllnessesRepository.Update(existingPatientIllness);
            await _patientIllnessesRepository.SaveAllAsync();
            return patientIllness;
        }

        public async Task<List<PatientIllnessesDTO>> GetAllPatientIllnessesForUserAsync(string userId)
        {
            var patientIllnesses = await _patientIllnessesRepository.FindAsync(x => x.PatientId == userId && !x.IsDeleted);
            return [..patientIllnesses.Select(x => new PatientIllnessesDTO
            {
                PatientId = x.PatientId,
                IllnessId = x.IllnessId,
                DiagnosisDate = x.DiagnosisDate,
                Notes = x.Notes
            })];
        }

        public async Task<PatientIllnessesDTO> GetPatientIllnessByIdAsync(int id, string userId)
        {
            var patientIllness = await _patientIllnessesRepository.GetByIdAsync(id, userId);
            if (patientIllness == null)
            {
                throw new Exception("Patient illness not found");
            }
            return new PatientIllnessesDTO
            {
                PatientId = patientIllness.PatientId,
                IllnessId = patientIllness.IllnessId,
                DiagnosisDate = patientIllness.DiagnosisDate,
                Notes = patientIllness.Notes
            };
        }

        private async Task<bool> Validation(PatientIllnessesDTO patientIllness)
        {
            if(await _patientIllnessesRepository.IsIdValidTypeAsync<Patient>(patientIllness.PatientId)  && await _patientIllnessesRepository.IsIdValidTypeAsync<Illness>(patientIllness.IllnessId)
            {
                return true;
            }
            return false;

        }
    }
}
