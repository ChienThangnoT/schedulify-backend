using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetSubjectListWithSchoolId(int schoolId, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(()=> _subjectService.GetSubjectBySchoolId(schoolId, includeDeleted, pageSize, pageIndex));
        }

        [HttpPost]
        [Authorize]
        public Task <IActionResult> CreateSubject(SubjectAddModel model)
        {
            return ValidateAndExecute(()=> _subjectService.CreateSubject(model));
        }
    }
}
