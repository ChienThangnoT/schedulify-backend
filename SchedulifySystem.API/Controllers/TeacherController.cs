using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/teachers")]
    [ApiController]
    public class TeacherController : BaseController
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetTeachers(bool includeDeleted = false, int pageSize = 20, int pageIndex = 1)
        {
            return ValidateAndExecute(() => _teacherService.GetTeachers(includeDeleted, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize]
        public Task<IActionResult> CreateTeacher(CreateTeacherRequestModel model)
        {
            return ValidateAndExecute(() => _teacherService.CreateTeacher(model));
        }

        [HttpPut("{id}")]
        [Authorize]
        public Task<IActionResult> UpdateTeacher(int id, UpdateTeacherRequestModel model)
        {
            return ValidateAndExecute(() => _teacherService.UpdateTeacher(id, model));
        }

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetTeacherById(int id)
        {
            return ValidateAndExecute(() => _teacherService.GetTeacherById(id));
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public Task<IActionResult> DeleteTeacher(int id)
        {
            return ValidateAndExecute(() => _teacherService.DeleteTeacher(id));
        }
    }
}
