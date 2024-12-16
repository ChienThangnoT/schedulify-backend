using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITeachableSubjectService
    {
        Task<BaseResponseModel> GetByTeacherId(int schoolId, int id);
        Task<BaseResponseModel> GetBySubjectId(int schoolId, TeacherStatus? teacherStatus, int id, EGrade eGrade);
    }
}
