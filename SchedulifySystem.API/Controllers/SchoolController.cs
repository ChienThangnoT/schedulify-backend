using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools")]
    [ApiController]
    public class SchoolController : BaseController
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet]
        public Task<IActionResult> GetSchools(SchoolStatus schoolStatus, int districtCode,  int provinceId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _schoolService.GetSchools(pageIndex, pageSize, districtCode, provinceId,schoolStatus));
        }
    }
}
