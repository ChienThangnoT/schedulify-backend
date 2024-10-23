using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/terms")]
    [ApiController]
    public class TermController : BaseController
    {
        private readonly ITermService _termService;

        public TermController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpGet("{schoolId}")]
        [Authorize]
        public Task<IActionResult> GetTermBySchoolId(int schoolId)
        {
            return ValidateAndExecute(() => _termService.GetTermBySchoolId(schoolId));
        }
    }
}
