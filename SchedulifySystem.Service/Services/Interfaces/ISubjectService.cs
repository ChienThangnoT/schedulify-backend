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
        Task<BaseResponseModel> CreateSubject(SubjectAddModel subjectAddModel);
        Task<BaseResponseModel> CreateSubjectList(int schoolId,List<SubjectAddListModel> subjectAddModel);
        Task<BaseResponseModel> GetSubjectBySchoolId(int schoolId, string? schoolName, bool includeDeleted, int pageSize, int pageIndex);
    }
}
