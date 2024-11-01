using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/timetables")]
    [ApiController]
    public class TimeTableController : BaseController
    {
        private readonly ITimetableService _timetableService;

        public TimeTableController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        [HttpPost]
        public async Task<IActionResult> GetT(GenerateTimetableModel parameters) {
            var result = await _timetableService.GetData(parameters);
            return Ok(result);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(GenerateTimetableModel parameters)
        {
            var result = await _timetableService.Generate(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public  Task<IActionResult> GetTimetable(int id)
        {
            return  ValidateAndExecute(() => _timetableService.Get(id));
        }
    }
}
