using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
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
        Task<BaseResponseModel> GetSubjects(int subjectId, int subjectGroupTypeId, int pageIndex, int pageSize);
        Task<BaseResponseModel> CreateSubjectGroup(int schoolId , SubjectGroupAddModel subjectGroupAddModel);
    }
}
