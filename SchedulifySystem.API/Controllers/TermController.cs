using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/academic-years/{yearId}/terms")]
    [ApiController]
    public class TermController : BaseController
    {
        private readonly ITermService _termService;

        public TermController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetTermBySchoolId(int schoolId)
        {
            return ValidateAndExecute(() => _termService.GetTermBySchoolId(schoolId));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CreateTermBySchoolId(int schoolId, TermAdjustModel termAddModel)
        {
            return ValidateAndExecute(() => _termService.AddTermBySchoolId(schoolId, termAddModel));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateTermBySchoolId(int id, TermAdjustModel termAddModel)
        {
            return ValidateAndExecute(() => _termService.UpdateTermBySchoolId(id, termAddModel));
        }
    }
}
