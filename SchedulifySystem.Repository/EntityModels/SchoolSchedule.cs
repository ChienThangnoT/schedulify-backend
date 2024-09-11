using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SchoolSchedule
    {
        public int SchoolScheduleId { get; set; }
        public int SchoolYearId { get; set; }
        public int SchoolId { get; set; }
        public int TeacherId { get; set; }
        public DateTime GenerateAt { get; set; }
        public DateTime ApplyDate { get; set; }
        public int WeeklyRange { get; set; }
        public string? Name { get; set; }
        public int ScheduleType { get; set; }
        public int MainSession { get; set; }
        public int TermId { get; set; }

        public School School { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public Term Term { get; set; }

        public ICollection<ScheduleConfig> ScheduleConfigs { get; set; }
        public ICollection<TeacherConfig> TeacherConfigs { get; set; }
        public ICollection<SubjectConfig> SubjectConfigs { get; set; }
        public ICollection<ClassSchedule> ClassSchedules { get; set; }
      
    }
}
