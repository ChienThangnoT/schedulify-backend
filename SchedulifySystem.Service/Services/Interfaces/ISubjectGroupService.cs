using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISubjectGroupService
    {
        Task<BaseResponseModel> GetSubjectGroups(int schoolId, int? subjectGroupId, Grade? grade, bool includeDeleted,int pageIndex, int pageSize);
        Task<BaseResponseModel> GetSubjectGroupDetail(int subjectGroupId);
        Task<BaseResponseModel> CreateSubjectGroup(int schoolId , SubjectGroupAddModel subjectGroupAddModel);
        Task<BaseResponseModel> UpdateSubjectGroup(int subjectGroupId, SubjectGroupUpdateModel subjectGroupUpdateModel);
        
    }
}
