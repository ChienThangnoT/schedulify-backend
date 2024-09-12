using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ClassSchedule : BaseEntity
    {
        public int Status { get; set; }
        public string? Name { get; set; }
        public int SchoolScheduleId { get; set; }

        public SchoolSchedule? SchoolSchedule { get; set; }
        public ICollection<ClassPeriod> ClassPeriods { get; set; } = new List<ClassPeriod>();
    }
}
