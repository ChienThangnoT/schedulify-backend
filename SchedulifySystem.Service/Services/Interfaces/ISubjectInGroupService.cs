using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISubjectInGroupService
    {
        Task<BaseResponseModel> UpdateSubjectInGroup(int schoolId, int yearId, int subjectGroupId, int termId, List<SubjectInGroupUpdateModel> subjectInGroupUpdateModel);
    }
}
