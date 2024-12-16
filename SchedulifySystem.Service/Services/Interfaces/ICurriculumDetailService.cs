using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ICurriculumDetailService
    {
        Task<BaseResponseModel> UpdateCurriculumDetail(int schoolId, int yearId, int curriculumId, int termId, List<CurriculumDetailUpdateModel> curriculumDetailUpdateModel);
    }
}
