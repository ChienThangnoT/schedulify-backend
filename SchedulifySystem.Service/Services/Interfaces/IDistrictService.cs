using SchedulifySystem.Service.BusinessModels.DistrictBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IDistrictService
    {
        Task<BaseResponseModel> GetDistrictByProvinceId(int? provinceId, int pageIndex, int pageSize);
        Task<BaseResponseModel> AddDistricts(int provinceId, List<DistrictAddModel> models);
        Task<BaseResponseModel> UpdateDistrict(int provinceId, int districtCode, DistrictUpdateModel model);
        Task<BaseResponseModel> DeleteDistrict(int provinceId, int districtCode);
    }
}
