using SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ICurriculumService
    {
        Task<BaseResponseModel> GetCurriculums(int schoolId, int? CurriculumId, EGrade? grade,int? schoolYearId, bool includeDeleted,int pageIndex, int pageSize);
        Task<BaseResponseModel> GetCurriculumDetails(int curriculumId);
        Task<BaseResponseModel> CreateCurriculum(int schoolId , CurriculumAddModel CurriculumAddModel);
        Task<BaseResponseModel> UpdateCurriculum(int CurriculumId, CurriculumUpdateModel CurriculumUpdateModel);
        Task<BaseResponseModel> DeleteCurriculum(int CurriculumId);
        Task<BaseResponseModel> QuickAssignPeriod(int schoolId, int schoolYearId, QuickAssignPeriodModel model);
        Task<BaseResponseModel> GetQuickAssignPeriodData(int schoolId, int schoolYearId);

    }
}
