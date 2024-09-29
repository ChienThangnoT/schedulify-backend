using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : BaseController
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet()]
        public Task<IActionResult> GetTeachers(bool includeDeleted = false, int pageSize = 20, int pageIndex = 1)
        {
            return ValidateAndExecute(() => _teacherService.GetTeachers(includeDeleted, pageIndex, pageSize));
        }

        [HttpPost()]
        public Task<IActionResult> CreateTeacher(CreateTeacherRequestModel model)
        {
            return ValidateAndExecute(() => _teacherService.CreateTeacher(model));
        }

        [HttpPut("{id}")]
        public Task<IActionResult> UpdateTeacher(int id, UpdateTeacherRequestModel model)
        {
            return ValidateAndExecute(() => _teacherService.UpdateTeacher(id, model));
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetTeacherById(int id)
        {
            return ValidateAndExecute(() => _teacherService.GetTeacherById(id));
        }
    }
}
