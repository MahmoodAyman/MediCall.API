using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interface
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(AppUser appUser);
        Task<RefreshTocken> GenerateRefreshTokenAsync();

    }
}
 