using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
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
        public Task<AuthenticationResponseModel> SignInAccountAsync(SignInModel signInModel);
        public Task<AuthenticationResponseModel> RefreshToken(string jwtToken);
        public Task<BaseResponseModel> CreateSchoolManagerAccount(CreateSchoolManagerModel createSchoolManagerModel);
    }
}
