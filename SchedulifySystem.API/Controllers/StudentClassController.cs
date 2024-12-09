using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/classes")]
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
        public Task<IActionResult> GetStudentClasses(int schoolId, int yearId, EGrade? grade = null, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClasses(schoolId, grade, yearId, includeDeleted, pageIndex, pageSize));
        }

        [HttpGet("subjects-in-group")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetSubjectInGroupOfClass(int schoolId, int termId, int studentClassId)
        {
            return ValidateAndExecute(() => _studentClassService.GetSubjectInGroupOfClass(schoolId, termId, studentClassId));
        }

        [HttpGet("{id}/assignments-in-class")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetTeacherAssignmentOfClass(int id, int yearId)
        {
            return ValidateAndExecute(() => _studentClassService.GetTeacherAssignmentOfClass(id, yearId));
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetStudentClassById(int id)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClassById(id));
        }


        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> CreateStudentClasses(int schoolId, int yearId, List<CreateListStudentClassModel> models)
        {
            return ValidateAndExecute(() => _studentClassService.CreateStudentClasses(schoolId, yearId, models));
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

        [HttpGet]
        [Route("class-combination")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> GetClassCombination(int schoolId, int yearId, int subjectId, int termId, EGrade grade, MainSession session)
        {
            return ValidateAndExecute(() => _studentClassService.GetClassCombination(schoolId, yearId, subjectId, termId, grade, session));
        }

    }
}
