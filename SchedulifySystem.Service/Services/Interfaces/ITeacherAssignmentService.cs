using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITeacherAssignmentService
    {
        Task<BaseResponseModel> GetAssignment(int classId,int? termId);
        Task<BaseResponseModel> CheckValidAssignment(int schoolId, int termId, List<TeacherAssignmentMinimalData> teacherAssignmentMinimalDatas);
        Task<BaseResponseModel> AssignTeacherForAsignments(List<AssignTeacherAssignmentModel> models);
        Task<BaseResponseModel> AutoAssignTeachers(int schoolId, int yearId, AutoAssignTeacherModel model);
        Task<BaseResponseModel> CheckTeacherAssignment(int schoolId, int yearId, AutoAssignTeacherModel model);
    }
}
