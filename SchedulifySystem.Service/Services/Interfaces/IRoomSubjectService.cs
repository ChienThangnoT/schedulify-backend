using SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IRoomSubjectService
    {
        public Task<BaseResponseModel> AddRoomSubject(RoomSubjectAddModel roomSubjectAddModel);
        public Task<BaseResponseModel> ViewRoomSubjectList(int schoolId, int? roomSubjectId,int? termId, int pageIndex, int pageSize);
    }
}
