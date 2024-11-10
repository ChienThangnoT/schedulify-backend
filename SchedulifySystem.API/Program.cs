using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.API;
using SchedulifySystem.API.Middleware;
using SchedulifySystem.Service.Hubs;
using SchedulifySystem.Service.Validations;
using SchedulifySystem.Service.ViewModels.ResponseModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(m => m.Value.Errors.Count > 0)
            .SelectMany(m => m.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = new BaseResponseModel
        {
            Status = StatusCodes.Status400BadRequest,
            Message = string.Join("; ", errors)
        };

        return new BadRequestObjectResult(response);
    };
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWebAPIService(builder);
builder.Services.AddInfractstructure(builder.Configuration);
builder.Services.AddSignalR();

var app = builder.Build();

// Configure Swagger to be available in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedulify Web API");
    c.RoutePrefix = "swagger";
});

// Use HTTPS Redirection only in Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapHub<NotificationHub>("/notificationHub");

app.UseCors("app-cors");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
