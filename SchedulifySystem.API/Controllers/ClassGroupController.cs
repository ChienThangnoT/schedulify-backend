using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/class-groups")]
    [ApiController]
    public class ClassGroupController : BaseController
    {
        private readonly IClassGroupService _classGroupService;

        public ClassGroupController(IClassGroupService classGroupService)
        {
            _classGroupService = classGroupService;
        }

        [HttpGet]
        [Route("grades")]
        [Authorize]
        public Task<IActionResult> GetGrades() 
        {
            return ValidateAndExecute(() => _classGroupService.GetGrades());
        }
    }
}
