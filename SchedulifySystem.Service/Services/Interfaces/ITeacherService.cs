using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<BaseResponseModel> GetTeachers(bool includeDeleted, int pageIndex, int pageSize);
        Task<BaseResponseModel> CreateTeacher(CreateTeacherRequestModel createTeacherRequestModel);
        Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherRequestModel updateTeacherRequestModel);
        Task<BaseResponseModel> GetTeacherById(int id);
        Task<BaseResponseModel> DeleteTeacher(int id);
    }
}
