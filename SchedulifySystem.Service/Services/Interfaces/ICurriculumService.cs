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
        Task<BaseResponseModel> GetCurriculums(int schoolId, EGrade? grade, int schoolYearId, int pageIndex, int pageSize);
        Task<BaseResponseModel> GetCurriculumDetails(int schoolId, int yearId, int curriculumId);
        Task<BaseResponseModel> CreateCurriculum(int schoolId, int schoolYearId, CurriculumAddModel CurriculumAddModel);
        Task<BaseResponseModel> UpdateCurriculum(int schoolId, int schoolYearId, int curriculumId, CurriculumUpdateModel curriculumUpdateModel);
        Task<BaseResponseModel> DeleteCurriculum(int schoolId, int yearId, int CurriculumId);
        Task<BaseResponseModel> QuickAssignPeriod(int schoolId, int schoolYearId, QuickAssignPeriodModel model);
        Task<BaseResponseModel> GetQuickAssignPeriodData(int schoolId, int schoolYearId);

    }
}
