using SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<BaseResponseModel> GetDepartments(int schoolId, int pageIndex = 1, int pageSize = 20);
        Task<BaseResponseModel> AddDepartment(int schoolId, List<DepartmentAddModel> models);
        Task<BaseResponseModel> UpdateRoom(int departmentId, DepartmentUpdateModel model);
        Task<BaseResponseModel> DeleteRoom(int departmentId);
    }
}
