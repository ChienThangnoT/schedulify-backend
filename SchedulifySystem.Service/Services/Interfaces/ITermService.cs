using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITermService
    {
        Task<BaseResponseModel> GetTerms(int? termId, int schoolYearId,int pageIndex, int pageSize);
        Task<BaseResponseModel> AddTerm(TermAdjustModel termAddModel);
        Task<BaseResponseModel> UpdateTermById(int termId, TermAdjustModel termAddModel);
    }
}
