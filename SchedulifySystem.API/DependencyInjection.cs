using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchedulifySystem.Repository.EntityModels;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;
using SchedulifySystem.Repository.DBContext;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Mapper;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Repository.Repositories.Implements;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.Services.Implements;


namespace SchedulifySystem.API
{
    public static class DependencyInjection
    {
        public static void AddWebAPIService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.KebabCaseLower;
                    option.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddEndpointsApiExplorer();

            #region add cors
            services.AddCors(options =>
            {
                options.AddPolicy("app-cors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithExposedHeaders("X-Pagination")
                        .AllowAnyMethod();
                    });
            });
            #endregion

            #region config authen swagger
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Schedulify Web API", Version = "v.10.24" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add Authentication and JwtBearer
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                    };
                });
            #endregion
        }
        public static IServiceCollection AddInfractstructure(this IServiceCollection services, IConfiguration config)
        {
            #region config service 

            //Configure UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(MapperConfigs).Assembly);

            //config user service and repo
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            //config teacher service and repo
            services.AddTransient<ITeacherRepository, TeacherRepository>();
            services.AddTransient<ITeacherService, TeacherService>();

            //config role service and repo
            services.AddTransient<IRoleRepository, RoleRepository>();
            //services.AddTransient<IRoleService, RoleService>();

            //config role assignmentt service and repo
            services.AddTransient<IRoleAssignmentRepository, RoleAssignmentRepository>();
            //services.AddTransient<IRoleAssignmentService, RoleAssignmentService>();
            #endregion

            #region add db context
            services.AddDbContext<SchedulifyContext>(options =>
                options.UseNpgsql(config.GetConnectionString("SchedulifyPostgres")));
            return services;
            #endregion
        }
    }
}
