using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponseModel> GetListAccount(AccountStatus? accountStatus, int pageIndex, int pageSize); 
    }
}
