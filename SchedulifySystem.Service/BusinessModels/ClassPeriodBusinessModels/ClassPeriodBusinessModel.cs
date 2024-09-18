using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels
{
    public class ClassPeriodBusinessModel : BaseEntity
    {
        public int ClassScheduleId { get; set; }
        public string? TimeSlotName { get; set; }
        public int TimeSlotId { get; set; }
        public string? RoomName { get; set; }
        public string? TeacherName { get; set; }
        public string? SubjectName { get; set; }
        public ClassPeriodStatus Status { get; set; }
    }
}
