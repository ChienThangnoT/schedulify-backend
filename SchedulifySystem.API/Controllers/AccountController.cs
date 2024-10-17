using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> GetAccounts(AccountStatus accountStatus, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _accountService.GetListAccount(accountStatus, pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetAccountDetails(int id)
        {
            return ValidateAndExecute(() => _accountService.GetAccountInformation(id));
        }

        [HttpPut("{id}")]
        [Authorize]
        public Task<IActionResult> UpdateAccountDetails(int id, UpdateAccountModel updateAccountModel)
        {
            return ValidateAndExecute(() => _accountService.UpdateAccountInformation(id, updateAccountModel));
        }

        [HttpPut("change-password")]
        public Task<IActionResult> ChangePasswrod(int id, ChangePasswordModel changePasswordModel)
        {
            return ValidateAndExecute(() => _accountService.ChangeAccountPassword(id, changePasswordModel));
        }
    }
}
