using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using System.Collections.Generic;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetSubjectBySubjectId(int id)
        {
            return ValidateAndExecute(() => _subjectService.GetSubjectById(id));
        }

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetSubjectListWithSchoolId(int schoolId, string? subjectName, bool? isRequired,bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(()=> _subjectService.GetSubjectBySchoolId(schoolId, subjectName, isRequired, includeDeleted, pageSize, pageIndex));
        }

        [HttpPost("{schoolId}/subjects")]
        [Authorize(Roles = "Admin, SchoolManager")]
        public Task<IActionResult> CreateSubjectList(int schoolId, List<SubjectAddListModel> subjectAddModel)
        {
            return ValidateAndExecute(() => _subjectService.CreateSubjectList(schoolId, subjectAddModel));
        }

        [HttpPut("{subjectId}")]
        [Authorize(Roles = "Admin, SchoolManager")]
        public Task<IActionResult> UpdateSubjectById(int subjectId, SubjectUpdateModel subjectUpdate)
        {
            return ValidateAndExecute(() => _subjectService.UpdateSubjectById(subjectId, subjectUpdate));
        }
    }
}
