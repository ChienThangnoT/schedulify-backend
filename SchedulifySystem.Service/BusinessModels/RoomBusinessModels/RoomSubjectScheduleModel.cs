using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class RoomSubjectScheduleModel
    {
        public int RoomId { get; set; }
        public string? Name { get; set; }
        public string? RoomCode { get; set; }
        public int MaxClassPerTime { get; set; }
        public List<int> TeachableSubjectIds { get; set; } = new List<int>();
    }
}
