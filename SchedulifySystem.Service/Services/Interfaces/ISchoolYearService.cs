using SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISchoolYearService
    {
        Task<BaseResponseModel> GetSchoolYear(bool includePrivate);
        Task<BaseResponseModel> AddSchoolYear(SchoolYearAddModel model);
        Task<BaseResponseModel> UpdateSchoolYear(int id, SchoolYearUpdateModel model);
        Task<BaseResponseModel> DeteleSchoolYear(int id);
        Task<BaseResponseModel> UpdatePublicStatus(int id, bool status);
    }
}
