using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Room : BaseEntity
    {
        public string? Name { get; set; }
        public int MaxClassPerTime { get; set; }
        public int BuildingId { get; set; }
        public int AvailabilityeStatus { get; set; }
        public string? RoomCode { get; set; }
        public int RoomType { get; set; }
        public Building? Building { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<ClassPeriod> ClassPeriods { get; set; } = new List<ClassPeriod>();
        public ICollection<RoomSubject> RoomSubjects { get; set; } = new List<RoomSubject>();

        public static explicit operator int(Room v)
        {
            throw new NotImplementedException();
        }
    }
}
