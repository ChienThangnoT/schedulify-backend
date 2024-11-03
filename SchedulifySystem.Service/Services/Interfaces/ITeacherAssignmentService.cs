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
        Task<BaseResponseModel> UpdateAssignment(int assignmentId);
        Task<BaseResponseModel> AssignTeacherForAsignments(List<AssignTeacherAssignmentModel> models);
    }
}
