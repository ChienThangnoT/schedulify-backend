using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IUserService
    {
        public Task<BaseResponeModel> SignInAccountAsync(SignInModel signInModel);

    }
}
