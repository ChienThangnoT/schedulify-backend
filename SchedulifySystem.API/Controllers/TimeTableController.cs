using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/timetables")]
    [ApiController]
    public class TimeTableController : BaseController
    {
        private readonly ITimetableService _timetableService;

        public TimeTableController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }


        [HttpPost("generate")]
        public async Task<IActionResult> Generate(int schoolId, GenerateTimetableModel parameters)
        {
            parameters.SchoolId = schoolId;
            var result = await _timetableService.Generate(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetTimetable(int id)
        {
            return ValidateAndExecute(() => _timetableService.Get(id));
        }

        [HttpGet]
        public Task<IActionResult> GetAllSchedules(int schoolId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _timetableService.GetAll(schoolId, pageIndex, pageSize));
        }
    }
}
