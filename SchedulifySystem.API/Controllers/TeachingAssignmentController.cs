using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/teacher-assignments")]
    [ApiController]
    public class TeachingAssignmentController : BaseController
    {
        private readonly ITeacherAssignmentService _teacherAssignmentService;

        public TeachingAssignmentController(ITeacherAssignmentService teacherAssignmentService)
        {
            _teacherAssignmentService = teacherAssignmentService;
        }

        [HttpPatch]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddTeacherAssignment(List<AssignTeacherAssignmentModel> models)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.AssignTeacherForAsignments(models));
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetAssignment(int studentClassId, int? termId)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.GetAssignment(studentClassId, termId));
        }
        
    }
}
