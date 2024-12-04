using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubmitRequest;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/submit-requests")]
    [ApiController]
    public class SubmitRequestController : BaseController
    {
        private readonly ISubmitRequestService _submitRequestService;
        private readonly IClaimsService _claimsService;

        public SubmitRequestController(ISubmitRequestService submitRequestService, IClaimsService claimsService)
        {
            _submitRequestService = submitRequestService;
            _claimsService = claimsService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> GetSubmitRequestOfSchoolAsync([Required]int schoolYearId, ERequestStatus? eRequestStatus)
        {
            var currentSchoolId = int.Parse(_claimsService.GetCurrentSchoolId);
            return ValidateAndExecute(() => _submitRequestService.GetSubmitRequestOfSchoolAsync(schoolYearId, currentSchoolId, eRequestStatus));
        }

        [HttpPost]
        [Authorize(Roles = "TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> SendApplicationAsync(ApplicationRequest applicationRequest)
        {

            var currentSchoolId = int.Parse(_claimsService.GetCurrentSchoolId);
            return ValidateAndExecute(() => _submitRequestService.SendApplicationAsync(currentSchoolId, applicationRequest));
        }
    }
}
