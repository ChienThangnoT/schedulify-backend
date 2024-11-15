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

        public CurriculumController(ICurriculumService subjectGroupService)
        {
            _curriculumService = subjectGroupService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectGroups(int schoolId, int yearId, int? subjectGroupId, EGrade? grade, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _curriculumService.GetCurriculums(schoolId, subjectGroupId, grade, yearId, includeDeleted, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectGroupDetail(int id)
        {
            return ValidateAndExecute(() => _curriculumService.GetCurriculumDetails(id));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddSubjectGroup(int schoolId, CurriculumAddModel subjectGroupAddModel)
        {
            return ValidateAndExecute(() => _curriculumService.CreateCurriculum(schoolId, subjectGroupAddModel));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateSubjectGroup(int id, CurriculumUpdateModel model)
        {
            return ValidateAndExecute(() => _curriculumService.UpdateCurriculum(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteSubjectGroup(int id)
        {
            return ValidateAndExecute(() => _curriculumService.DeleteCurriculum(id));
        }

        [HttpPatch("quick-assign-period")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> QuickAssignPeriod(int schoolId, int yearId, QuickAssignPeriodModel model)
        {
            return ValidateAndExecute(() => _curriculumService.QuickAssignPeriod(schoolId, yearId, model));
        }

        [HttpGet("quick-assign-period-data")]
        public Task<IActionResult> GetQuickAssignPeriodData(int schoolId, int yearId)
        {
            return ValidateAndExecute(() => _curriculumService.GetQuickAssignPeriodData(schoolId, yearId));

        }
    }
}
