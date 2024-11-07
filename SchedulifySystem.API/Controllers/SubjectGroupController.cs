using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/subject-groups")]
    [ApiController]
    public class SubjectGroupController : BaseController
    {
        private readonly ISubjectGroupService _subjectGroupService;

        public SubjectGroupController(ISubjectGroupService subjectGroupService)
        {
            _subjectGroupService = subjectGroupService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectGroups(int schoolId, int yearId, int? subjectGroupId, EGrade? grade, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _subjectGroupService.GetSubjectGroups(schoolId, subjectGroupId, grade, yearId, includeDeleted, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectGroupDetail(int id)
        {
            return ValidateAndExecute(() => _subjectGroupService.GetSubjectGroupDetail(id));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddSubjectGroup(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            return ValidateAndExecute(() => _subjectGroupService.CreateSubjectGroup(schoolId, subjectGroupAddModel));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateSubjectGroup(int id, SubjectGroupUpdateModel model)
        {
            return ValidateAndExecute(() => _subjectGroupService.UpdateSubjectGroup(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteSubjectGroup(int id)
        {
            return ValidateAndExecute(() => _subjectGroupService.DeleteSubjectGroup(id));
        }

        [HttpPut]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> QuickAssignPeriod(int schoolId, int yearId, QuickAssignPeriodModel model)
        {
            return ValidateAndExecute(() => _subjectGroupService.QuickAssignPeriod(schoolId, yearId, model));
        }
    }
}
