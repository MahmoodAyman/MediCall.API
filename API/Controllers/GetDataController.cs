using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDataController : ControllerBase
    {
        private readonly IGetDataService _getDataService;
        public GetDataController(IGetDataService getDataService)
        {
            _getDataService = getDataService;
        }
        [HttpGet("certificates")]
        public async Task<IActionResult> GetAllCertificates()
        {
            var certificates = await _getDataService.GetAllCertificatesAsync();
            return Ok(certificates);
        }
        [HttpGet("illnesses")]
        public async Task<IActionResult> GetAllIllnesses()
        {
            var illnesses = await _getDataService.GetAllIllnessesAsync();
            return Ok(illnesses);
        }
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _getDataService.GetAllServicesAsync();
            return Ok(services);
        }
    }
}
