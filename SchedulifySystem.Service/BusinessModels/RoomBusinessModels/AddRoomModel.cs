using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class AddRoomModel
    {
        public string? Name { get; set; }
        public string? RoomType { get; set; }
        public int MaxClassPerTime { get; set; }
        public string? Building { get; set; }
        public AvailabilityStatus AvailabilityeStatus { get; set; }
    }
}
