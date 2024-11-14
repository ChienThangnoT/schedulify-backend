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

        public Room? Room { get; set; }
        public Subject? Subject { get; set; }
        public ICollection<StudentClassRoomSubject> StudentClassRoomSubjects { get; set; } = new List<StudentClassRoomSubject>();

    }
}
