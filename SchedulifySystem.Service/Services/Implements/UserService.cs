using AutoMapper;
using FTravel.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        private async Task<List<Claim>> GetAuthClaims(Account user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _unitOfWork.RoleAssignmentRepo.GetRolesByAccountIdAsync(user.Id);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.Role.Name ));
            }

            return authClaims;
        }

        private async Task<AuthenticationResponseModel> GenerateAuthenticationResponse(Account account)
        {
            var authClaims = await GetAuthClaims(account);
            var token = GenerateJWTToken.CreateAccessToken(authClaims, _configuration, DateTime.UtcNow);
            var refreshToken = GenerateJWTToken.CreateRefreshToken(authClaims, _configuration, DateTime.UtcNow).ToString();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            return new AuthenticationResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successfully!",
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expired = TimeZoneInfo.ConvertTimeFromUtc(token.ValidTo, TimeZoneInfo.Local),
                JwtRefreshToken = refreshToken,
                AccountId = account.Id
            };
        }

        public Task<BaseResponseModel> SignInAccountAsync(SignInModel signInModel)
        {
            throw new NotImplementedException();
        }
    }
}
