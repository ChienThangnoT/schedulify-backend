using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/subject-groups")]
    [ApiController]
    public class SubjectGroupController : BaseController
    {
        private readonly ISubjectGroupService _subjectGroupService;

        public SubjectGroupController(ISubjectGroupService subjectGroupService)
        {
            _subjectGroupService = subjectGroupService;
        }

        [HttpGet]
        [Authorize("Admin, SchoolManager")]
        public Task<IActionResult> GetSubjectGroups(int schoolId, int? subjectGroupId, Grade? grade, bool includeDeleted = false, int pageIndex =1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _subjectGroupService.GetSubjectGroups(schoolId, subjectGroupId, grade, includeDeleted, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize("Admin, SchoolManager")]
        public Task<IActionResult> GetSubjectGroupDetail(int id)
        {
            return ValidateAndExecute(() => _subjectGroupService.GetSubjectGroupDetail(id));
        }

        [HttpPost]
        [Authorize("Admin,SchoolManager")]
        public Task<IActionResult> AddSubjectGroup(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            return ValidateAndExecute(() => _subjectGroupService.CreateSubjectGroup(schoolId, subjectGroupAddModel));
        }
    }
}
