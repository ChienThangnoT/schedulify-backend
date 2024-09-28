using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.ViewModels.ResponseModels;

namespace SchedulifySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected async Task<IActionResult> ValidateAndExecute(Func<Task<BaseResponseModel>> func)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return GetActionResponse(await func());
        }


        protected IActionResult GetActionResponse(BaseResponseModel baseResponse)
        {
            switch (baseResponse.Status)
            {
                case StatusCodes.Status401Unauthorized:
                    {
                        return Unauthorized(baseResponse);
                    }
                case StatusCodes.Status200OK:
                    {
                        return Ok(baseResponse);
                    }
                case StatusCodes.Status400BadRequest:
                    {
                        return BadRequest(baseResponse);
                    }
                case StatusCodes.Status404NotFound:
                    {
                        return NotFound(baseResponse);
                    }
                case StatusCodes.Status500InternalServerError:
                    {
                        return BadRequest(baseResponse);
                    }
                case StatusCodes.Status409Conflict:
                    {
                        return Conflict(baseResponse);
                    }
                default:
                    {
                        return StatusCode(baseResponse.Status, baseResponse);
                    }
            };
        }
    }
}
