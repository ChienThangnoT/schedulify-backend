using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/academic-years/{schoolYearId}/terms")]
    [ApiController]
    public class TermController : BaseController
    {
        private readonly ITermService _termService;

        public TermController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpGet]
        public Task<IActionResult> GetTerms(int? termId, int schoolYearId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _termService.GetTerms(termId, schoolYearId, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> CreateTerm(TermAdjustModel termAddModel)
        {
            return ValidateAndExecute(() => _termService.AddTerm(termAddModel));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> UpdateTermById(int id, TermAdjustModel termAddModel)
        {
            return ValidateAndExecute(() => _termService.UpdateTermById(id, termAddModel));
        }
    }
}
