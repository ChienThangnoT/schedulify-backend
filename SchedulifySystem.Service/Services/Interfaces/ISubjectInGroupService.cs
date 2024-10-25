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
        Task<BaseResponseModel> GetSubjectInGroup(int schoolId, int? termId, int schoolYearId, int? subjectGroupId, int? subbjectInGroupId, int pageIndex, int pageSize);
    }
}
