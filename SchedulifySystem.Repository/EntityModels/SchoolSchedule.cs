using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SchoolSchedule : BaseEntity
    {
        public int SchoolYearId { get; set; }
        public int SchoolId { get; set; }
        public int TermId { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string? Name { get; set; }
        public int ScheduleType { get; set; }
        public int FitnessPoint {  get; set; }

        public School? School { get; set; }
        public SchoolYear? SchoolYear { get; set; }
        public Term? Term { get; set; }
        public ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
    }
}
