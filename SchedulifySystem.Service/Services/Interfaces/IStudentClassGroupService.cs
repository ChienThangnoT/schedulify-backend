using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IStudentClassGroupService
    {
        Task<BaseResponseModel> GetStudentClassGroups(int schoolId, int schoolYearId, int pageIndex = 1, int pageSize = 20);
        Task<BaseResponseModel> GetStudentClassGroupDetail(int schoolId, int schoolYearId, int studentClassGroupId);
        Task<BaseResponseModel> AddStudentClassgroup(int schoolId, int schoolYearId, List<AddStudentClassGroupModel> models);
        Task<BaseResponseModel> UpdateStudentClassGroup(int classGroupId, UpdateStudentClassGroupModel model);
        Task<BaseResponseModel> DeleteStudentClassGroup(int classGroupId);
        Task<BaseResponseModel> AssignCurriculumToClassGroup(int schoolId, int schoolYearId, int id, int curriculumId);
        Task<BaseResponseModel> AssignClassToClassGroup(int schoolId, int schoolYearId, int id, AssignClassToClassGroup model);
    }
}
