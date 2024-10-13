using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISchoolService
    {
        Task<BaseResponseModel> GetSchools(int pageIndex, int pageSize, int? districtCode, int? provinceId, SchoolStatus? schoolStatus);
    }
}
