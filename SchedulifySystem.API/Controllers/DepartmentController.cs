using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/departments")]
    [ApiController]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetDepartment(int schoolId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _departmentService.GetDepartments(schoolId, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddDepartment(int schoolId, List<DepartmentAddModel> models)
        {
            return ValidateAndExecute(() => _departmentService.AddDepartment(schoolId, models));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateDepartment(int id, int schoolId, DepartmentUpdateModel model)
        {
            return ValidateAndExecute(() => _departmentService.UpdateDepartment(id, schoolId, model));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteDepartemnt(int id)
        {
            return ValidateAndExecute(() => _departmentService.DeleteDepartment(id));
        }

        [HttpPost("generate-teacher")]
        public Task<IActionResult> GenerateTeacherAccount(GenerateTeacherInDepartmentAccountModel generateModel)
        {
            return ValidateAndExecute(() => _departmentService.GenerateDepartmentAccount(generateModel));
        }

    }
}
