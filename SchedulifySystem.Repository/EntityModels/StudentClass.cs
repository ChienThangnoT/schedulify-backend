using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class StudentClass : BaseEntity
    {
        public string? Name { get; set; }
        public int? HomeroomTeacherId { get; set; }
        public int SchoolId { get; set; }
        public int? RoomId { get; set; }
        public int SchoolYearId { get; set; }
        public int MainSession { get; set; }
        public bool IsFullDay { get; set; } = false;
        public int PeriodCount { get; set; }
        public int Grade { get; set; }
        public int? StudentClassGroupId { get; set; }

        public Room? Room { get; set; }
        public School? School { get; set; }
        public Teacher? Teacher { get; set; }
        public SchoolYear? SchoolYear { get; set; }
        public StudentClassGroup? StudentClassGroup { get; set; }

        public ICollection<StudentClassRoomSubject> StudentClassRoomSubjects { get; set; } = new List<StudentClassRoomSubject>();
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
        public ICollection<SubjectConfig> SubjectConfigs { get; set; } = new List<SubjectConfig>();
        public ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
    }
}
