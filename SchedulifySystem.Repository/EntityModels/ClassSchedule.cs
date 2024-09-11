using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class ClassSchedule
    {
        public int ClassScheduleId { get; set; }
        public int SchoolScheduleId { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
