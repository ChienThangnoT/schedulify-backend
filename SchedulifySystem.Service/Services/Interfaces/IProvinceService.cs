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
    }
}
