using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Models;

namespace Core.Interface
{
    public interface IAuthService
    {
        Task<AuthDTO> PatientRegisterAsync(RegisterDTO registerDTO);
        Task<AuthDTO> NurseRegisterAsync(NurseRegisterDTO registerDTO);
        Task<AuthDTO> LoginAsync(LoginDTO loginDTO);
        Task<AuthDTO> RefreshTokenAsync(string token);
        Task<AuthDTO> ConfirmEmailAsync(string email, string token);
    }
}
