using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.HolidayBusinessModels
{
    public class HolidayBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public HolidayType HolidayType { get; set; }

        public bool IsLongHoliday()
        {
            return (EndTime - StartTime).TotalDays > 7;
        }

        public bool IsValidHoliday()
        {
            return StartTime < EndTime;
        }

        public bool IsNationalHoliday()
        {
            return HolidayType == HolidayType.NationalHoliday;
        }

        public bool IsSchoolHoliday()
        {
            return HolidayType == HolidayType.SchoolHoliday;
        }

        public bool IsExamBreak()
        {
            return HolidayType == HolidayType.ExamBreak;
        }

        public bool IsSummerBreak()
        {
            return HolidayType == HolidayType.SummerBreak;
        }
    }
}
