using Newtonsoft.Json;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.ViewModels.ResponseModels;

namespace SchedulifySystem.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AccountAlreadyExistsException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status409Conflict);
            }
            catch (AlreadyExistsException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status409Conflict);
            }
            catch (NotExistsException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound);
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new BaseResponseModel
            {
                Status = statusCode,
                Message = exception.Message,
            };

            var jsonResponse = JsonConvert.SerializeObject(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
