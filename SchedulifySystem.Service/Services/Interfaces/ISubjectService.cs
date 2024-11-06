using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<BaseResponseModel> GetSubjectById(int subjectId);
        Task<BaseResponseModel> CreateSubjectList(int schoolYearId, List<SubjectAddListModel> subjectAddModel);// need update subject group
        Task<BaseResponseModel> GetSubjectById(int schoolYearId, int? id, string? subjectName, bool? isRequired, bool includeDeleted, int pageSize, int pageIndex);
        Task<BaseResponseModel> UpdateSubjectById(int subjectId, SubjectUpdateModel subjectUpdateModel);
        Task<BaseResponseModel> DeleteSubjectById(int subjectId);

    }
}
