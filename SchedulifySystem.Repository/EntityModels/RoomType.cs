using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }

        public ICollection<Room> Rooms { get; set; }
        public School School { get; set; }
    }
}
