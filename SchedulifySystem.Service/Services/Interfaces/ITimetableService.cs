using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITimetableService
    {
        public Task<BaseResponseModel> Generate(GenerateTimetableModel paraModel);
        public Task<BaseResponseModel> Get(int id);
        public Task<BaseResponseModel> Check(Guid timetableId);
        public Task<BaseResponseModel> Update(TimetableIndividual timetable);
        public Task<BaseResponseModel> Delete(int id);
        public Task<(
            List<ClassScheduleModel>,
            List<TeacherScheduleModel>,
            List<SubjectScheduleModel>,
            List<TeacherAssigmentScheduleModel>,
            ETimetableFlag[,]
            )> GetData(GenerateTimetableModel parameters);
    }
}
