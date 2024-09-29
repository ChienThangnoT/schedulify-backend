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
        public Task<IActionResult> GetTeachers(int pageSize = 20, int pageIndex = 1)
        {
            return ValidateAndExecute(() => _teacherService.GetTeachers(pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize("Admin")]
        public Task<IActionResult> CreateTeacher(CreateTeacherRequestModel model)
        {
            return ValidateAndExecute(() => _teacherService.CreateTeacher(model));
        }
    }
}
