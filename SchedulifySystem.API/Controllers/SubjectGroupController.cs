using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
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

        [HttpPost]
        [Authorize("Admin")]
        public Task<IActionResult> AddSubjectGroup(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            return ValidateAndExecute(() => _subjectGroupService.CreateSubjectGroup(schoolId, subjectGroupAddModel));
        }
    }
}
