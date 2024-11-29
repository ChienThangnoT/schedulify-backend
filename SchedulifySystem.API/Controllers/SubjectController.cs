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
        public Task<IActionResult> GetSubjectListWithSchoolYearId(int schoolYearIdint, int? id, string? subjectName, bool? isRequired,bool includeDeleted = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(()=> _subjectService.GetSubjectById(schoolYearIdint, id, subjectName, isRequired, includeDeleted, pageSize, pageIndex));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> CreateSubjectList(int schoolYearId, List<SubjectAddListModel> subjectAddModel)
        {
            return ValidateAndExecute(() => _subjectService.CreateSubjectList(schoolYearId, subjectAddModel));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> UpdateSubjectById(int id, SubjectUpdateModel subjectUpdate)
        {
            return ValidateAndExecute(() => _subjectService.UpdateSubjectById(id, subjectUpdate));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> DeleteSubjectById(int id)
        {
            return ValidateAndExecute(() => _subjectService.DeleteSubjectById(id));
        }
    }
}
