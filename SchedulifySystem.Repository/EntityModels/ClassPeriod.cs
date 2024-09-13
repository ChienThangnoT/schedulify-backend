using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ClassPeriod : BaseEntity
    {
        public int ClassScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public int RoomId { get; set; }
        public int Status { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }

        public TimeSlot? TimeSlot { get; set; }
        public ClassSchedule? ClassSchedule { get; set; }
        public Room? Room { get; set; }
        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
    }
}
