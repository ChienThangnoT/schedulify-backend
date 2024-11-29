using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/room-subjects")]
    [ApiController]
    public class RoomSubjectController : BaseController
    {
        private readonly IRoomSubjectService _roomSubjectService;

        public RoomSubjectController(IRoomSubjectService roomSubjectService)
        {
            _roomSubjectService = roomSubjectService;
        }

        [HttpGet]
        public Task<IActionResult> GetRoomSubjects(int schoolId, int? roomSubjectId, int? termId, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _roomSubjectService.ViewRoomSubjectList(schoolId, roomSubjectId, termId, pageIndex, pageSize));
        }

        [HttpPost]
        public Task<IActionResult> AddRoomSubject(RoomSubjectAddModel roomSubjectAddModel)
        {
            return ValidateAndExecute(() => _roomSubjectService.AddRoomSubject(roomSubjectAddModel));
        }

        [HttpPut("{id}")]
        public Task<IActionResult> UpdateRoomSubject(int schoolId, int id, RoomSubjectUpdateModel model)
        {
            return ValidateAndExecute(() => _roomSubjectService.UpdateRoomSubject(schoolId, id, model));
        }
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteRoomSubject(int schoolId, int id)
        {
            return ValidateAndExecute(() => _roomSubjectService.DeleteRoomSubject(schoolId, id));
        }
    }
}
