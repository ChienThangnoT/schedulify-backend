using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/teachers")]
    [ApiController]
    public class TeacherController : BaseController
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetTeachers(int schoolId, bool includeDeleted = false, int pageSize = 20, int pageIndex = 1)
        {
            return ValidateAndExecute(() => _teacherService.GetTeachers(schoolId, includeDeleted, pageIndex, pageSize));
        }
        

        [HttpGet("assignment")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetTeacherAssignmentDetail(int teacherId, int schoolYearId)
        {
            return ValidateAndExecute(() => _teacherService.GetTeacherAssignmentDetail(teacherId, schoolYearId));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CreateTeachers(int schoolId, List<CreateListTeacherModel> models)
        {
            return ValidateAndExecute(() => _teacherService.CreateTeachers(schoolId, models));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateTeacher(int id, UpdateTeacherModel model)
        {
            return ValidateAndExecute(() => _teacherService.UpdateTeacher(id, model));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetTeacherById(int id)
        {
            return ValidateAndExecute(() => _teacherService.GetTeacherById(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteTeacher(int id)
        {
            return ValidateAndExecute(() => _teacherService.DeleteTeacher(id));
        }

        [HttpPatch("assign-department-head")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AssignDepartmentHead(int schoolId, List<AssignTeacherDepartmentHeadModel> models)
        {
            return ValidateAndExecute(() => _teacherService.AssignTeacherDepartmentHead(schoolId, models));
        }

        [HttpPost("generate-account")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> GenerateTeacherAccount(TeacherGenerateAccount teacherGenerateAccount)
        {
            return ValidateAndExecute(() => _teacherService.GenerateTeacherAccount(teacherGenerateAccount));
        }


    }
}
