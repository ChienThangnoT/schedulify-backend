using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class TeacherConfig : BaseEntity
    {
        public int ConfigAttributeId { get; set; }
        public int SchoolScheduleId { get; set; }
        public int TeacherId { get; set; }
        public int? Value { get; set; }

        public ConfigAttribute? ConfigAttribute { get; set; }
        public Teacher? Teacher { get; set; }
        public SchoolSchedule? SchoolSchedule { get; set; }
    }
}
