﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/student-classes")]
    [ApiController]
    public class StudentClassController : BaseController
    {
        private readonly IStudentClassService _studentClassService;

        public StudentClassController(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetStudentClasses(int schoolId, EGrade? grade = null, int? schoolYearId = null, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClasses(schoolId, grade, schoolYearId, includeDeleted, pageIndex, pageSize));
        }

        [HttpGet("subjects-in-group")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectInGroupOfClass(int schoolId, int termId, int studentClassId)
        {
            return ValidateAndExecute(() => _studentClassService.GetSubjectInGroupOfClass(schoolId, termId, studentClassId));
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetStudentClassById(int id)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClassById(id));
        }


        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            return ValidateAndExecute(() => _studentClassService.CreateStudentClasses(schoolId, schoolYearId, models));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteStudentClass(int id)
        {
            return ValidateAndExecute(() => _studentClassService.DeleteStudentClass(id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateStudentClass(int id, UpdateStudentClassModel model)
        {
            return ValidateAndExecute(() => _studentClassService.UpdateStudentClass(id, model));
        }

        [HttpPatch()]
        [Route("assign-homeroom-teacher")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AssignHomeroomTeacherToClasses(AssignListStudentClassModel models)
        {
            return ValidateAndExecute(() => _studentClassService.AssignHomeroomTeacherToClasses(models));
        }

        [HttpPatch()]
        [Route("assign-subject-group")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AssignSubjectGroupToClasses(AssignSubjectGroup model)
        {
            return ValidateAndExecute(() => _studentClassService.AssignSubjectGroupToClasses(model));
        }

    }
}
