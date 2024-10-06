using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
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
        [Authorize]
        public Task<IActionResult> GetStudentClasses(int schoolId, int? gradeId = null, int? schoolYearId = null, bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClasses(schoolId, gradeId, schoolYearId, includeDeleted, pageIndex, pageSize));
        }


        [HttpGet("{classGroupId}")]
        [Authorize]
        public Task<IActionResult> GetStudentClassById(int classGroupId)
        {
            return ValidateAndExecute(() => _studentClassService.GetStudentClassById(classGroupId));
        }

        [HttpPost]
        [Authorize]
        public Task<IActionResult> CreateStudentClass(CreateStudentClassModel model)
        {
            return ValidateAndExecute(() => _studentClassService.CreateStudentClass(model));
        }

        [HttpPost]
        [Route("add-list")]
        [Authorize]
        public Task<IActionResult> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            return ValidateAndExecute(() => _studentClassService.CreateStudentClasses(schoolId, schoolYearId, models));
        }

        [HttpDelete("{classGroupId}")]
        [Authorize]
        public Task<IActionResult> DeleteStudentClass(int classGroupId)
        {
            return ValidateAndExecute(() => _studentClassService.DeleteStudentClass(classGroupId));
        }

        [HttpPut("{classGroupId}")]
        [Authorize]
        public Task<IActionResult> UpdateStudentClass(int classGroupId, UpdateStudentClassModel model)
        {
            return ValidateAndExecute(() => _studentClassService.UpdateStudentClass(classGroupId, model));
        }

    }
}
