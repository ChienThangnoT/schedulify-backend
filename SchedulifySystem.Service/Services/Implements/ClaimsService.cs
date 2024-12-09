using Microsoft.AspNetCore.Http;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class ClaimsService : IClaimsService
    {
        private readonly string _currentUserEmail;
        private readonly string _currentSchoolId;

        public ClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

            if (identity == null)
            {
                _currentUserEmail = null;
                _currentSchoolId = null;
                return;
            }

            _currentUserEmail = ClaimsUtils.GetEmailFromIdentity(identity);
            _currentSchoolId = ClaimsUtils.GetSchoolIdFromIdentity(identity);
        }

        public string GetCurrentUserEmail => _currentUserEmail;
        public string GetCurrentSchoolId => _currentSchoolId;
    }
}
