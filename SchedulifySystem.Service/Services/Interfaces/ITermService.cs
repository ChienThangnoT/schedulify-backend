﻿using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
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
        Task<BaseResponseModel> GetTermBySchoolId(int schoolId);
        Task<BaseResponseModel> AddTermBySchoolId(int schoolId, TermAdjustModel termAddModel);
        Task<BaseResponseModel> UpdateTermBySchoolId(int termId, TermAdjustModel termAddModel);
    }
}
