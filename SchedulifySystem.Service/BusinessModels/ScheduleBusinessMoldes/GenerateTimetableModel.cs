using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class GenerateTimetableModel
    {
        public int SchoolYearId;
        public List<ClassPeriod> FixedPeriods { get; set; } = new List<ClassPeriod>(); // ds tiết cố định 
        public List<ClassPeriod> NoAssignTimetablePeriods { get; set; } = new List<ClassPeriod>(); //ds tiết không xếp 
        public List<ClassPeriod> BusyTimetablePeriods { get; set; } = new List<ClassPeriod>(); // ds tiết bận của gv 

        public int MaxPeriodPerSession { get; set; } = 5;
        public int MinPeriodPerSession { get; set; } = 0;
        public Term? Term { get; set; }
    }
}
