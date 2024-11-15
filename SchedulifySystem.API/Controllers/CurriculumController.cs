using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/curriculum")]
    [ApiController]
    public class CurriculumController : BaseController
    {
        private readonly ICurriculumService _curriculumService;

        public CurriculumController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetCurriculums(int schoolId, int yearId, EGrade? grade, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _curriculumService.GetCurriculums(schoolId, grade, yearId, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetCurriculumDetail(int schoolId, int yearId, int id)
        {
            return ValidateAndExecute(() => _curriculumService.GetCurriculumDetails(schoolId, yearId, id));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddCurriculum(int schoolId, int yearId, CurriculumAddModel curriculumAddModel)
        {
            return ValidateAndExecute(() => _curriculumService.CreateCurriculum(schoolId, yearId, curriculumAddModel));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateCurriculum(int schoolId, int yearId, int id, CurriculumUpdateModel model)
        {
            return ValidateAndExecute(() => _curriculumService.UpdateCurriculum(schoolId, yearId, id, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteCurriculum(int schoolId, int yearId, int id)
        {
            return ValidateAndExecute(() => _curriculumService.DeleteCurriculum(schoolId, yearId, id));
        }

        [HttpPatch("quick-assign-period")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> QuickAssignPeriod(int schoolId, int yearId, QuickAssignPeriodModel model)
        {
            return ValidateAndExecute(() => _curriculumService.QuickAssignPeriod(schoolId, yearId, model));
        }

        [HttpGet("quick-assign-period-data")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> GetQuickAssignPeriodData(int schoolId, int yearId)
        {
            return ValidateAndExecute(() => _curriculumService.GetQuickAssignPeriodData(schoolId, yearId));

        }
    }
}
