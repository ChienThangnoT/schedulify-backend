using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/subject-groups/{subjectGroupId}/subject-in-groups")]
    [ApiController]
    public class SubjectInGroupController : BaseController
    {
        private ISubjectInGroupService _subjectInGroupService;

        public SubjectInGroupController(ISubjectInGroupService subjectInGroupService)
        {
            _subjectInGroupService = subjectInGroupService;
        }

        [HttpPatch]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateTimeSlot(List<SubjectInGroupUpdateModel> subjectInGroupUpdateModel)
        {
            return ValidateAndExecute(() => _subjectInGroupService.UpdateSubjectInGroup(subjectInGroupUpdateModel));
        }
    }
}
