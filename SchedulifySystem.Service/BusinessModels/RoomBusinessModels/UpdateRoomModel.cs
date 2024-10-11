using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class UpdateRoomModel
    {
        public string Name { get; set; }
        public int RoomTypeId { get; set; }
        public int MaxClassPerTime { get; set; }
        public int BuildingId { get; set; }
        public AvailabilityStatus AvailabilityeStatus { get; set; }
    }
}
