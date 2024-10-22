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
        public async Task<IActionResult> Get(GenerateTimetableModel parameters) {
            var result =  _timetableService.GetData(parameters);
            return Ok(result);
        }
    }
}
