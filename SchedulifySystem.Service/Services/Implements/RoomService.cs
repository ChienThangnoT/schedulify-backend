using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class RoomService : IRoomService
    {
        public Task<BaseResponseModel> AddRooms(int schoolId, List<AddRoomModel> models)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> CheckValidDataAddRooms(int schoolId, List<AddRoomModel> models)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> DeleteRoom(int RoomId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> GetRooms(int schoolId, int buildingId, int pageIndex = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> UpdateRoom(int RoomId, UpdateRoomModel model)
        {
            throw new NotImplementedException();
        }
    }
}
