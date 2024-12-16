using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.SubmitRequest;
using SchedulifySystem.Service.BusinessModels.SubmitRequestBusinessModels;
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
        public Task<IActionResult> GetSubmitRequestOfSchoolAsync([Required] int schoolYearId, ERequestStatus? eRequestStatus)
        {
            var currentSchoolId = int.Parse(_claimsService.GetCurrentSchoolId);
            return ValidateAndExecute(() => _submitRequestService.GetSubmitRequestOfSchoolAsync(schoolYearId, currentSchoolId, eRequestStatus));
        }

        [HttpGet("teacher/{id}")]
        [Authorize(Roles = "Teacher")]
        public Task<IActionResult> GetSubmitRequestByTeacherId(int id, [Required]int schoolYearId, ERequestStatus? eRequestStatus)
        {
            return ValidateAndExecute(() => _submitRequestService.GetSubmitRequestByTeacherId(id, schoolYearId, eRequestStatus));
        }

        [HttpPost]
        [Authorize(Roles = "TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> SendApplicationAsync(SubmitSendRequestModel applicationRequest)
        {

            var currentSchoolId = int.Parse(_claimsService.GetCurrentSchoolId);
            return ValidateAndExecute(() => _submitRequestService.SendApplicationAsync(currentSchoolId, applicationRequest));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> ProcessSubmitRequest(int id, [Required][FromBody]ProcessSubmitRequestModel processSubmitRequestModel)
        {
            return ValidateAndExecute(() => _submitRequestService.ProcessSubmitRequest(id, processSubmitRequestModel));
        }
    }
}
