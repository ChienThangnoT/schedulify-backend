using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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

        [HttpPatch("check-valid-assignments")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CheckValidAssignment(int schoolId, [Required]int termId, [Required][FromBody]List<TeacherAssignmentMinimalData> teacherAssignmentMinimalDatas)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.CheckValidAssignment(schoolId, termId, teacherAssignmentMinimalDatas));
        }

        [HttpPatch("auto-assign-teacher")]
        //[Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AutoAssignTeachers(int schoolId, int yearId,AutoAssignTeacherModel model)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.AutoAssignTeachers(schoolId, yearId, model));
        }

        [HttpPatch("check-auto-assign-teacher")]
        //[Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CheckAutoAssignTeacher(int schoolId, int yearId, AutoAssignTeacherModel model)
        {
            return ValidateAndExecute(() => _teacherAssignmentService.CheckTeacherAssignment(schoolId, yearId, model));
        }


    }
}
