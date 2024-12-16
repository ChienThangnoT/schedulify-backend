using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class StudentClassRoomSubject : BaseEntity
    {
        public int StudentClassId { get; set; }
        public int RoomSubjectId { get; set; }

        public RoomSubject? RoomSubject { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}
