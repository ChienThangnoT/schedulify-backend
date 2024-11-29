using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<BaseResponseModel> GetTeachers(int schoolId,int? departmentId, bool includeDeleted, int pageIndex, int pageSize);
        Task<BaseResponseModel> CreateTeacher(CreateTeacherModel createTeacherRequestModel);
        Task<BaseResponseModel> CreateTeachers(int schoolId, List<CreateListTeacherModel> createTeacherRequestModels);
        Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherModel updateTeacherRequestModel);
        Task<BaseResponseModel> AddTeachableSubjects(int id, List<SubjectGradeModel> teachableSubjects);
        Task<BaseResponseModel> GetTeacherById(int id);
        Task<BaseResponseModel> DeleteTeacher(int id);
        Task<BaseResponseModel> DeleteTeachableSubjeect(int teachableSubjectId);
        Task<BaseResponseModel> GenerateTeacherAccount(TeacherGenerateAccount teacherGenerateAccount);
        Task<BaseResponseModel> AssignTeacherDepartmentHead(int schoolId, List<AssignTeacherDepartmentHeadModel> models);
        Task<BaseResponseModel> GetTeacherAssignmentDetail(int teacherId, int schoolYearId);
    }
}
