﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/academic-years/{yearId}/class-groups")]
    [ApiController]
    public class StudentClassGroupController : BaseController
    {
        private readonly IStudentClassGroupService _studentClassGroupService;

        public StudentClassGroupController(IStudentClassGroupService studentClassGroupService)
        {
            _studentClassGroupService = studentClassGroupService;
        }

        [HttpGet]
        private Task<IActionResult> GetStudentClassGroups(int schoolId, int schoolYearId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _studentClassGroupService.GetStudentClassGroups(schoolId, schoolYearId, pageIndex, pageSize));
        }

        [HttpDelete("{id}")]
        private Task<IActionResult> DeleteById(int id)
        {
            return ValidateAndExecute(() => _studentClassGroupService.DeleteStudentClassGroup(id));
        }

        [HttpPost]
        private Task<IActionResult> AddStudentClassGroups(int schoolId, int schoolYearId, List<AddStudentClassGroupModel> models)
        {
            return ValidateAndExecute(() => _studentClassGroupService.AddStudentClassgroup(schoolId, schoolYearId, models));
        }

        [HttpPut("{id}")]
        private Task<IActionResult> UpdateStudentClassGroups(int id, UpdateStudentClassGroupModel model)
        {
            return ValidateAndExecute(() => _studentClassGroupService.UpdateStudentClassGroup(id, model));
        }

    }
}