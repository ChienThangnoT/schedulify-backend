using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ClassPeriod : BaseEntity
    {
        public int? ClassScheduleId { get; set; }
        public int? RoomId { get; set; }
        public int? Status { get; set; }
        public int? TeacherId { get; set; }
        public int? SubjectId { get; set; }
        public int? DateOfWeek { get; set; }
        public string? SubjectAbbreviation { get; set; }
        public string? TeacherAbbreviation { get; set; }
        public string? RoomCode { get; set; }
        public int? TeacherAssignmentId {  get; set; }
        public int StartAt { get; set; }
        public int Priority { get; set; } 

        public ClassSchedule? ClassSchedule { get; set; }
        public Room? Room { get; set; }
        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
        public ICollection<PeriodChange> PeriodChanges { get; set; } = new List<PeriodChange>();
    }
}
