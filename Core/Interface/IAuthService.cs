using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interface
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> AuthenticateAsync(AppUser appUser);
        Task<(string newAccessToken, string newRefreshToken)> RefreshTokenAsync(string token);

    }
}
