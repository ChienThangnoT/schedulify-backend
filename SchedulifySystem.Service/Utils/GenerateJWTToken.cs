using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FTravel.Service.Utils
{
    public static class GenerateJWTToken
    {
        private static JwtSecurityToken CreateJwtToken(List<Claim> authClaims, IConfiguration configuration, DateTime currentTime, TimeSpan tokenValidity)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: currentTime.Add(tokenValidity),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        public static JwtSecurityToken CreateAccessToken(List<Claim> authClaims, IConfiguration configuration, DateTime currentTime)
        {
            _ = int.TryParse(configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            TimeSpan tokenValidity = TimeSpan.FromMinutes(tokenValidityInMinutes);
            return CreateJwtToken(authClaims, configuration, currentTime, tokenValidity);
        }

        public static JwtSecurityToken CreateRefreshToken(List<Claim> authClaims, IConfiguration configuration, DateTime currentTime)
        {
            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int tokenValidityInDays);
            TimeSpan tokenValidity = TimeSpan.FromDays(tokenValidityInDays);
            return CreateJwtToken(authClaims, configuration, currentTime, tokenValidity);
        }
    }
}