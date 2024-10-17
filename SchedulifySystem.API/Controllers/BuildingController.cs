using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/buildings")]
    [ApiController]
    public class BuildingController : BaseController
    {

        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager")]
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

        [HttpPut("{buildingId}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateBuilding(int buildingId, UpdateBuildingModel building)
        {
            return ValidateAndExecute(() => _buildingService.UpdateBuildings(buildingId, building));
        }

        [HttpDelete("{buildingId}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteBuilding(int buildingId)
        {
            return ValidateAndExecute(() => _buildingService.DeleteBuildings(buildingId));
        }

    }
}
