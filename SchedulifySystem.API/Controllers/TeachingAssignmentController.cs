using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingAssignmentController : BaseController
    {
        private readonly ITeacherAssignmentService _teacherAssignmentService;

        public TeachingAssignmentController(ITeacherAssignmentService teacherAssignmentService)
        {
            _teacherAssignmentService = teacherAssignmentService;
        }

        [HttpPost]
        public Task<IActionResult> AddTeacherAssignment(AddTeacherAssignmentModel model)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.AddAssignment(model));
        }
        
    }
}
