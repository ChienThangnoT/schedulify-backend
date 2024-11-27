using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/provinces")]
    [ApiController]
    public class ProvinceController : BaseController
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService  provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public Task<IActionResult> GetDistrictByProvinceId(int? id)
        {
            return ValidateAndExecute(() => _provinceService.GetProvinces(id));
        }
    }
}
