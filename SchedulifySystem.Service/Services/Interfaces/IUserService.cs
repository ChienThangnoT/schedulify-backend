﻿using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponseModel> SignInAccountAsync(SignInModel signInModel);
        Task<AuthenticationResponseModel> RefreshToken(string jwtToken);
        Task<BaseResponseModel> CreateSchoolManagerAccount(CreateSchoolManagerModel createSchoolManagerModel);
        Task<BaseResponseModel> CreateAdminAccount(CreateAdmin createSchoolManagerModel);
        Task<BaseResponseModel> ConfirmCreateSchoolManagerAccount(int schoolManagerId, int schoolId, AccountStatus accountStatus);
        Task<BaseResponseModel> RequestResetPassword(string gmail);
        Task<BaseResponseModel> ConfirmResetPassword(string gmail, int code);
        Task<BaseResponseModel> ExcuteResetPassword(ResetPasswordModel resetPasswordModel);
    }
}
