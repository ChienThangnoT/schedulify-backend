using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public Task<IActionResult> AddRooms(int schoolId, List<AddRoomModel> models)
        {
            return ValidateAndExecute(() => _roomService.AddRooms(schoolId, models));
        }

        [HttpGet]
        public Task<IActionResult> GetRooms(int schoolId, int? buildingId,int? RoomTypeId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _roomService.GetRooms(schoolId, buildingId,RoomTypeId, pageIndex, pageSize));
        }

        
    }
}
