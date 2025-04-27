using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController(IServiceService _serviceService) : ControllerBase
    {
        [HttpGet("get-services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            if (services == null)
            {
                return BadRequest();
            }
            return Ok(services);
        }
    }
}
