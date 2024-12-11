using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.ProvinceBusinessModels;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/provinces")]
    [ApiController]
    public class ProvinceController : BaseController
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public Task<IActionResult> GetDistrictByProvinceId(int? id, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _provinceService.GetProvinces(id, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> AddProvinces(List<ProvinceAddModel> models)
        {
            return ValidateAndExecute(() => _provinceService.AddProvinces(models));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> UpdateProvince(int id, ProvinceUpdateModel model)
        {
            return ValidateAndExecute(() => _provinceService.UpdateProvince(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> RemoveProvince(int id)
        {
            return ValidateAndExecute(() => _provinceService.RemoveProvince(id));
        }

    }
}
