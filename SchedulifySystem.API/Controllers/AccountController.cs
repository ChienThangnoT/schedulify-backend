using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize]
        public Task<IActionResult> GetAccounts(AccountStatus accountStatus, int pageIndex = 1, int pageSize = 20)
        {
            return ValidateAndExecute(() => _accountService.GetListAccount(accountStatus, pageIndex, pageSize));
        }
    }
}
