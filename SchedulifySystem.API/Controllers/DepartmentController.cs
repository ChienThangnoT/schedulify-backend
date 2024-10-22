using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public Task<IActionResult> GetDepartment(int schoolId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _departmentService.GetDepartments(schoolId, pageIndex, pageSize));
        }

        [HttpPost]
        public Task<IActionResult> AddDepartment(int schoolId, List<DepartmentAddModel> models)
        {
            return ValidateAndExecute(() => _departmentService.AddDepartment(schoolId, models));
        }

    }
}
