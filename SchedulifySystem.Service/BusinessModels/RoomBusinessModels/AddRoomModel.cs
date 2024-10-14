using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class AddRoomModel
    {
        public string? Name { get; set; }
        public string? RoomCode { get; set; }
        public string? RoomTypeCode { get; set; }
        public int MaxClassPerTime { get; set; }
        public string? BuildingCode { get; set; }

        [JsonIgnore]
        public int? buildingId { get; set; }
        [JsonIgnore]
        public int? RoomTypeId { get; set; }
        [JsonIgnore]
        public AvailabilityStatus AvailabilityeStatus { get; set; } = AvailabilityStatus.Available;
    }
}
