using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/academic-years")]
    [ApiController]
    public class SchoolYearController : BaseController
    {
        private readonly ISchoolYearService _schoolYearService;

        public SchoolYearController(ISchoolYearService schoolYearService)
        {
            _schoolYearService = schoolYearService;
        }

        [HttpGet]
        public Task<IActionResult> GetSchoolYears(bool? includePrivate = false)
        {
            includePrivate = includePrivate ?? false;
            return ValidateAndExecute(() => _schoolYearService.GetSchoolYear((bool)includePrivate));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> AddSchoolYear(SchoolYearAddModel model)
        {
            return ValidateAndExecute(() => _schoolYearService.AddSchoolYear(model));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> UpdateSchoolYear(int id, SchoolYearUpdateModel model)
        {
            return ValidateAndExecute(() => _schoolYearService.UpdateSchoolYear(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> DeleteSchoolYear(int id)
        {
            return ValidateAndExecute(() => _schoolYearService.DeteleSchoolYear(id));
        }

        [HttpPatch("{id}/update-public-status")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> DeleteSchoolYear(int id, bool status)
        {
            return ValidateAndExecute(() => _schoolYearService.UpdatePublicStatus(id, status));
        }
    }
}
