using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class GenerateScheduleModel
    {
        public List<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public List<SubjectScheduleModel> SubjectSchedules { get; set; } = new List<SubjectScheduleModel>();
        public List<ClassPeriod> FixedPeriods { get; set; } = new List<ClassPeriod>(); 
        public List<ClassPeriod> NoAssignTimetablePeriods { get; set; } = new List<ClassPeriod>(); 
        public List<ClassPeriod> BusyTimetablePeriods { get; set; } = new List<ClassPeriod>();
        public int MaxPeriodPerSession { get; set; } = 5;
        public int MinPeriodPerSession { get; set; } = 0;
        public Term Term { get; set; }
    }
}
