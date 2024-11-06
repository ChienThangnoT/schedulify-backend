using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/buildings")]
    [ApiController]
    public class BuildingController : BaseController
    {

        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetBuildings(int schoolId, bool includeRoom = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _buildingService.GetBuildings(schoolId, includeRoom, pageIndex, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddBuildings(int schoolId, List<AddBuildingModel> buildings)
        {
            return ValidateAndExecute(() => _buildingService.AddBuildings(schoolId, buildings));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateBuilding(int id, UpdateBuildingModel building)
        {
            return ValidateAndExecute(() => _buildingService.UpdateBuildings(id, building));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteBuilding(int id)
        {
            return ValidateAndExecute(() => _buildingService.DeleteBuildings(id));
        }

    }
}
