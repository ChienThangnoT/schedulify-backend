using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}")]
    [ApiController]
    public class TeachableSubjectController : BaseController
    {
        private readonly ITeachableSubjectService _service;

        public TeachableSubjectController(ITeachableSubjectService service)
        {
            _service = service;
        }

        [HttpGet("teachers/{teacherId}/teachable-subjects")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetByTeacherId(int schoolId, int teacherId)
        {
            return ValidateAndExecute(() => _service.GetByTeacherId(schoolId, teacherId));
        }

        [HttpGet("subjects/{subjectId}/teachable-subjects")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetBySubjectId(int schoolId, int subjectId)
        {
            return ValidateAndExecute(() => _service.GetBySubjectId(schoolId, subjectId));
        }
    }
}
