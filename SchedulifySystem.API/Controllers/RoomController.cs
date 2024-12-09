using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/schools/{schoolId}/rooms")]
    [ApiController]
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> AddRooms(int schoolId, List<AddRoomModel> models)
        {
            return ValidateAndExecute(() => _roomService.AddRooms(schoolId, models));
        }

        [HttpGet]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetRooms(int schoolId, int? capicity,int? buildingId,ERoomType? RoomTypeId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _roomService.GetRooms(schoolId, capicity, buildingId,RoomTypeId, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SchoolManager, TeacherDepartmentHead, Teacher")]
        public Task<IActionResult> GetRoomById(int id)
        {
            return ValidateAndExecute(() => _roomService.GetRoomById(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> DeleteRoom(int id)
        {
            return ValidateAndExecute(() => _roomService.DeleteRoom(id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolManager")]
        public Task<IActionResult> UpdateRoom(int id, UpdateRoomModel model)
        {
            return ValidateAndExecute(() => _roomService.UpdateRoom(id, model));
        }
    }
}
