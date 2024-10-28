using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/school-years")]
    [ApiController]
    public class SchoolYearController : BaseController
    {
        private readonly ISchoolYearService _schoolYearService;

        public SchoolYearController(ISchoolYearService schoolYearService)
        {
            _schoolYearService = schoolYearService;
        }

        [HttpGet]
        public Task<IActionResult> GetSchoolYears()
        {
            return ValidateAndExecute(() => _schoolYearService.GetSchoolYear());
        }
    }
}
