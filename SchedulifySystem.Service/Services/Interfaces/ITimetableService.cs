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
        Task<BaseResponseModel> Generate(GenerateTimetableModel paraModel);
        Task<BaseResponseModel> Get(int id);
        Task<BaseResponseModel> GetAll(int schoolId, int pageIndex, int pageSize);
        Task<BaseResponseModel> Check(Guid timetableId);
        Task<BaseResponseModel> Update(TimetableIndividual timetable);
        Task<BaseResponseModel> Delete(int id);
        Task<(
            List<ClassScheduleModel>,
            List<TeacherScheduleModel>,
            List<SubjectScheduleModel>,
            List<TeacherAssigmentScheduleModel>,
            ETimetableFlag[,]
            )> GetData(GenerateTimetableModel parameters);
        Task<BaseResponseModel> CheckPeriodChange(CheckPeriodChangeModel model);
        Task<BaseResponseModel> UpdateStatusTimeTable(int schoolId, int yearId,int termId ,ScheduleStatus scheduleStatus);
        //Task<BaseResponseModel> PublishedTimetable(int schoolId, int yearId,int termId ,ScheduleStatus scheduleStatus);
    }
}
