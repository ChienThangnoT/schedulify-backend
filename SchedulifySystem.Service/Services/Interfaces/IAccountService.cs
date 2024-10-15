using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
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
        Task<BaseResponseModel> GetAccountInformation(int accountId); 
        Task<BaseResponseModel> UpdateAccountInformation(int accountId, UpdateAccountModel updateAccountModel); 
        Task<BaseResponseModel> ChangeAccountPassword(int accountId, ChangePasswordModel changePasswordModel); 
    }
}
