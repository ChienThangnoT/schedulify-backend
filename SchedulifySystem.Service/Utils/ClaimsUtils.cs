using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils
{
    public static class ClaimsUtils
    {
        public static string GetEmailFromIdentity(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return userClaims.FirstOrDefault(x => x.Type == "email")?.Value;
            }
            return null;
        }

        public static string GetSchoolIdFromIdentity(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return userClaims.FirstOrDefault(x => x.Type == "schoolId")?.Value;
            }
            return null;
        }
    }
}
