using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.ViewModels.ResponseModels;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.SignInAccountAsync(signInModel);
                    if (result.Status == StatusCodes.Status200OK)
                    {
                        return Ok(result);
                    }
                    return Unauthorized(result);
                }
                return ValidationProblem(ModelState);
            }
            catch (Exception ex)
            {
                var resp = new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(resp);
            }
        }
    }
}
