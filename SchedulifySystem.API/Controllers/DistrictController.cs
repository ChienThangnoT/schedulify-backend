using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.DistrictBusinessModels;
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

        [HttpGet("provinces/{provinceId}")]
        public Task<IActionResult> GetDistrictByProvinceId(int provinceId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _districtService.GetDistrictByProvinceId(provinceId, pageIndex, pageSize));
        }

        [HttpPost("provinces/{provinceId}")]
        public Task<IActionResult> AddDistricts(int provinceId, List<DistrictAddModel> models)
        {
            return ValidateAndExecute(() => _districtService.AddDistricts(provinceId,models));
        }

        [HttpPut("/{districtCode}/provinces/{provinceId}")]
        public Task<IActionResult> UpdateDistricts(int provinceId, int districtCode,DistrictUpdateModel model)
        {
            return ValidateAndExecute(() => _districtService.UpdateDistrict(provinceId, districtCode,model));
        }

        [HttpDelete("/{districtCode}/provinces/{provinceId}")]
        public Task<IActionResult> DeleteDistricts(int provinceId, int districtCode)
        {
            return ValidateAndExecute(() => _districtService.DeleteDistrict(provinceId, districtCode));
        }
    }
}
