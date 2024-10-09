using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : BaseController
    {

        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet]
        public Task<IActionResult> GetBuildings(int schoolId, bool includeRoom = false, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _buildingService.GetBuildings(schoolId, includeRoom, pageIndex, pageSize));
        }

        [HttpPost]
        public Task<IActionResult> AddBuildings(int schoolId, List<AddBuildingModel> buildings)
        {
            return ValidateAndExecute(() => _buildingService.AddBuildings(schoolId, buildings));
        }

       
    }
}
