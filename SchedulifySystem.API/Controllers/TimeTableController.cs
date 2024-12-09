using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/timetables")]
    [ApiController]
    public class TimeTableController : BaseController
    {
        private readonly ITimetableService _timetableService;
        private readonly IClaimsService _claimsService;

        public TimeTableController(ITimetableService timetableService, IClaimsService claimsService)
        {
            _timetableService = timetableService;
            _claimsService = claimsService;
        }


        [HttpPost("generate")]
        public async Task<IActionResult> Generate(int schoolId,int yearId, GenerateTimetableModel parameters)
        {
            var currentEmail = _claimsService.GetCurrentUserEmail;
            parameters.CurrentUserEmail = currentEmail;
            parameters.SchoolId = schoolId;
            parameters.SchoolYearId = yearId;
            var result = await _timetableService.Generate(parameters);
            return Ok(result);
        }

        [HttpGet("{day:datetime}")]
        public Task<IActionResult> GetTimetable(int schoolId, [Required]int termId, DateTime day)
        {
            return ValidateAndExecute(() => _timetableService.Get(schoolId,  termId, day));
        }

        [HttpGet]
        public Task<IActionResult> GetAllSchedules(int schoolId, int yearId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _timetableService.GetAll(schoolId, yearId,pageIndex, pageSize));
        }
        
        [HttpGet("available-teachers")]
        public Task<IActionResult> GetTeacherScheduleByWeek(int schoolId, [FromQuery]GetTeacherInSlotModel getTeacherInSlotModel)
        {
            return ValidateAndExecute(() => _timetableService.GetTeacherScheduleInWeek(schoolId, getTeacherInSlotModel));
        }

        [HttpPut("check-period-change")]
        public Task<IActionResult> CheckPeriodChange(CheckPeriodChangeModel model)
        {
            return ValidateAndExecute(() => _timetableService.CheckPeriodChange(model));
        }

        [HttpPut("status")]
        public Task<IActionResult> UpdateTimeTableStatus(int schoolId, int yearId, [Required]int termId, [Required] ScheduleStatus scheduleStatus)
        {
            return ValidateAndExecute(() => _timetableService.UpdateTimeTableStatus(schoolId, yearId, termId, scheduleStatus));
        }

        [HttpPost("publish")]
        public Task<IActionResult> PublishedTimetable(SchoolScheduleDetailsViewModel schoolScheduleDetailsModel)
        {
            return ValidateAndExecute(() => _timetableService.PublishedTimetable(schoolScheduleDetailsModel));
        }

    }
}
