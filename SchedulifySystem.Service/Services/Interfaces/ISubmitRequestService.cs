using SchedulifySystem.Service.BusinessModels.SubmitRequest;
using SchedulifySystem.Service.BusinessModels.SubmitRequestBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ISubmitRequestService
    {
        Task<BaseResponseModel> SendApplicationAsync(int schoolId , SubmitSendRequestModel applicationRequest);
        Task<BaseResponseModel> GetSubmitRequestOfSchoolAsync(int schoolYearId, int schoolId, ERequestStatus? eRequestStatus);
        Task<BaseResponseModel> GetSubmitRequestByTeacherId(int teacherId, int schoolYearId, ERequestStatus? eRequestStatus);
        Task<BaseResponseModel> ProcessSubmitRequest(int id, ProcessSubmitRequestModel processSubmitRequestModel);
    }
}
