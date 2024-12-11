using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils
{
    public static class TimeUtils
    {
        public static DateTime ConvertToLocalTime(DateTime utcTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
        }
    }
}
