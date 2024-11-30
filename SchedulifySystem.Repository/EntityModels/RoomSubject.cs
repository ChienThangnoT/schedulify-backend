using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class RoomSubject : BaseEntity
    {
        public int? SubjectId { get; set; }
        public int? RoomId { get; set; }
        public int? SchoolId { get; set; }
        public int? TermId { get; set; }
        public string? RoomSubjectCode { get; set; }
        public string? RoomSubjectName { get; set; }
        public int Model { get; set; }
        public int? Session {  get; set; }
        public int? SlotPerWeek {  get; set; }

        public Room? Room { get; set; }
        public Subject? Subject { get; set; }
        public School? School { get; set; }
        public Term? Term { get; set; }
        public ICollection<StudentClassRoomSubject> StudentClassRoomSubjects { get; set; } = new List<StudentClassRoomSubject>();
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();

    }
}
