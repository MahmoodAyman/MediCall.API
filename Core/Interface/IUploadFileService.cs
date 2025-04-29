using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Interface
{
    public interface IUploadFileService
    {
        Task<string> UploadFile(IFormFile file);
    }
}
