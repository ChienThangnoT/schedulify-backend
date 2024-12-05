using SchedulifySystem.Service.BusinessModels.ProvinceBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IProvinceService
    {
        Task<BaseResponseModel> GetProvinces(int? provinceId, int pageIndex, int PageSize);
        Task<BaseResponseModel> AddProvinces(List<ProvinceAddModel> models);
        Task<BaseResponseModel> RemoveProvince(int provinceId);
        Task<BaseResponseModel> UpdateProvince(int id, ProvinceUpdateModel model);
    }
}
