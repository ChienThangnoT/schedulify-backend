using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/curriculums/{subjectGroupId}/curriculum-details")]
    [ApiController]
    public class CurriculumDetailController : BaseController
    {
        private ICurriculumDetailService _curriculumDetailService;

        public CurriculumDetailController(ICurriculumDetailService subjectInGroupService)
        {
            _curriculumDetailService = subjectInGroupService;
        }

        [HttpPatch]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateTimeSlot(int schoolId, int yearId,int subjectGroupId,int termId,  List<CurriculumDetailUpdateModel> subjectInGroupUpdateModel)
        {
            return ValidateAndExecute(() => _curriculumDetailService.UpdateCurriculumDetail(schoolId, yearId, subjectGroupId, termId, subjectInGroupUpdateModel));
        }
    }
}
