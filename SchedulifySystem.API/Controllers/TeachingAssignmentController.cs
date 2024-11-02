using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/teacher-assignments")]
    [ApiController]
    public class TeachingAssignmentController : BaseController
    {
        private readonly ITeacherAssignmentService _teacherAssignmentService;

        public TeachingAssignmentController(ITeacherAssignmentService teacherAssignmentService)
        {
            _teacherAssignmentService = teacherAssignmentService;
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddTeacherAssignment(AddTeacherAssignmentModel model)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.AddAssignment(model));
        }

        [HttpGet]
        //[Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> GetAssignment(int studentClassId, int? termId)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.GetAssignment(studentClassId, termId));
        }
        
    }
}
