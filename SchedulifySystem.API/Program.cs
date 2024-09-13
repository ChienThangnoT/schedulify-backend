using SchedulifySystem.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddWebAPIService(builder);
builder.Services.AddInfractstructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedulify Web API");
});

app.UseHttpsRedirection();

app.UseCors("app-cors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
