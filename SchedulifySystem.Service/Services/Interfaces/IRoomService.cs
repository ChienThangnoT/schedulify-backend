using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IRoomService
    {
        Task<BaseResponseModel> GetRooms(int schoolId, int buildingId, int pageIndex = 1, int pageSize = 20);
        Task<BaseResponseModel> AddRooms(int schoolId, List<AddRoomModel> models);
        Task<BaseResponseModel> CheckValidDataAddRooms(int schoolId, List<AddRoomModel> models);
        Task<BaseResponseModel> UpdateRoom(int RoomId, UpdateRoomModel model);
        Task<BaseResponseModel> DeleteRoom(int RoomId);
    }
}
