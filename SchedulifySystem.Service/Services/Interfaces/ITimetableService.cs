using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
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
        public Task<BaseResponseModel> Get(Guid id);
        public Task<BaseResponseModel> Check(Guid timetableId);
        public Task<BaseResponseModel> Update(TimetableIndividual timetable);
        public Task<BaseResponseModel> Delete(Guid id);
    }
}
