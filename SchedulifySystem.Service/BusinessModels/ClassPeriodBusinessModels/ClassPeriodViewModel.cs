using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels
{
    public class ClassPeriodViewModel : BaseEntity
    {
        public int? ClassScheduleId { get; set; }
        public int? RoomId { get; set; }
        public string? RoomCode { get; set; }
        public int? TeacherId { get; set; }
        public int? SubjectId { get; set; }
        public int? DateOfWeek { get; set; }
        public string? SubjectAbbreviation { get; set; }
        public string? TeacherAbbreviation { get; set; }
        //public int? TeacherAssignmentId { get; set; }
        public int StartAt { get; set; }
        public EPriority Priority { get; set; }
    }
}
