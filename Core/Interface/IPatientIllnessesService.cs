using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;

namespace Core.Interface
{
    public interface IPatientIllnessesService
    {
        Task<List<PatientIllnessesDTO>> GetAllPatientIllnessesForUserAsync(string userId);
        Task<PatientIllnessesDTO> GetPatientIllnessByIdAsync(int id,string userId);
        Task<PatientIllnessesDTO> AddPatientIllnessAsync(PatientIllnessesDTO patientIllness);
        Task<PatientIllnessesDTO> UpdatePatientIllnessAsync(PatientIllnessesDTO patientIllness);
        Task DeletePatientIllnessAsync(int id,string userId);
    }
}
