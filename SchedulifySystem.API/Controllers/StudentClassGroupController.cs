using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/class-groups")]
    [ApiController]
    public class StudentClassGroupController : BaseController
    {
        private readonly IStudentClassGroupService _studentClassGroupService;

        public StudentClassGroupController(IStudentClassGroupService studentClassGroupService)
        {
            _studentClassGroupService = studentClassGroupService;
        }

        [HttpGet]
        public Task<IActionResult> GetStudentClassGroups(int schoolId, int yearId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _studentClassGroupService.GetStudentClassGroups(schoolId, yearId, pageIndex, pageSize));
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteById(int id)
        {
            return ValidateAndExecute(() => _studentClassGroupService.DeleteStudentClassGroup(id));
        }

        [HttpPost]
        public Task<IActionResult> AddStudentClassGroups(int schoolId, int yearId, List<AddStudentClassGroupModel> models)
        {
            return ValidateAndExecute(() => _studentClassGroupService.AddStudentClassgroup(schoolId, yearId, models));
        }

        [HttpPut("{id}")]
        public Task<IActionResult> UpdateStudentClassGroups(int id, UpdateStudentClassGroupModel model)
        {
            return ValidateAndExecute(() => _studentClassGroupService.UpdateStudentClassGroup(id, model));
        }

        [HttpPatch()]
        [Route("{id}/assign-curriculum/{curriculumId}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AssignSubjectGroupToClasses(int schoolId, int yearId, int id, int curriculumId)
        {
            return ValidateAndExecute(() => _studentClassGroupService.AssignCurriculumToClassGroup(schoolId,yearId,id,curriculumId));
        }

        [HttpPatch()]
        [Route("{id}/assign-class-to-class-group")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AssignSubjectGroupToClasses(int schoolId, int yearId, int id, AssignClassToClassGroup model)
        {
            return ValidateAndExecute(() => _studentClassGroupService.AssignClassToClassGroup(schoolId, yearId, id, model));
        }
    }
}
