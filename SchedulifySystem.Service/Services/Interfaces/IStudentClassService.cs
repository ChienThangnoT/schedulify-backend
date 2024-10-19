using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IStudentClassService
    {
        Task<BaseResponseModel> GetStudentClasses(int schoolId, EGrade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize);
        Task<BaseResponseModel> GetStudentClassById(int id);
        Task<BaseResponseModel> DeleteStudentClass(int id);
        Task<BaseResponseModel> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> createStudentClassModels);
        Task<BaseResponseModel> UpdateStudentClass(int id, UpdateStudentClassModel updateStudentClassModel);
        Task<BaseResponseModel> AssignHomeroomTeacherToClasses(AssignListStudentClassModel assignListStudentClassModel);
        Task<BaseResponseModel> GetSubjectInGroupOfClass(int schoolId, int termId, int studentClassId);
    }
}
