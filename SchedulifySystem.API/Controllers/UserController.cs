using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System.Drawing.Printing;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("confirm-school-manager-account")]
        [Authorize(Roles ="Admin")]
        public Task<IActionResult> ConfirmCreateSchoolManagerAccount(int schoolManagerId, int schoolId, AccountStatus accountStatus)
        {
            return ValidateAndExecute(() => _userService.ConfirmCreateSchoolManagerAccount(schoolManagerId, schoolId, accountStatus));
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
                var res = new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(res);
            }
        }

        [HttpPost("school-manager-register")]
        public async Task<IActionResult> SignupAccountManager(CreateSchoolManagerModel createSchoolManagerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userService.CreateSchoolManagerAccount(createSchoolManagerModel);
                    return Ok(result);
                }
                return ValidationProblem(ModelState);

            }
            catch (Exception ex)
            {
                var res = new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(res);

            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefresToken(string refreshToken)
        {
            try
            {
                var result = await _userService.RefreshToken(refreshToken);
                if (result.Status == StatusCodes.Status200OK)
                {
                    return Ok(result);
                }
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                var res = new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ex.Message.ToString()
                };
                return BadRequest(res);
            }
        }

        [HttpPost("reset-password")]
        public Task<IActionResult> RequestResetPassword(string email)
        {
            return ValidateAndExecute(() => _userService.RequestResetPassword(email));
        }

        [HttpPost("reset-password/confirm")]
        public Task<IActionResult> ConfirmResetPassword(string email, int code)
        {
            return ValidateAndExecute(() => _userService.ConfirmResetPassword(email, code));
        }
        
        [HttpPost("reset-password/new-password")]
        public Task<IActionResult> ConfirmResetPassword(ResetPasswordModel resetPasswordModel)
        {
            return ValidateAndExecute(() => _userService.ExcuteResetPassword(resetPasswordModel));
        }


        //[HttpPost("admin-register")]
        //public async Task<IActionResult> SignupAccountAdmin(CreateAdmin createSchoolManagerModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var result = await _userService.CreateAdminAccount(createSchoolManagerModel);
        //            return Ok(result);
        //        }
        //        return ValidationProblem(ModelState);

        //    }
        //    catch (Exception ex)
        //    {
        //        var res = new BaseResponseModel()
        //        {
        //            Status = StatusCodes.Status400BadRequest,
        //            Message = ex.Message.ToString()
        //        };
        //        return BadRequest(res);

        //    }
        //}

    }
}
