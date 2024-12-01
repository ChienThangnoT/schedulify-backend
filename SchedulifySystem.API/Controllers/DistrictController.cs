using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/districts")]
    [ApiController]
    public class DistrictController : BaseController
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet("provinces/{id}")]
        public Task<IActionResult> GetDistrictByProvinceId(int id, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _districtService.GetDistrictByProvinceId(id, pageIndex, pageSize));
        }
    }
}
