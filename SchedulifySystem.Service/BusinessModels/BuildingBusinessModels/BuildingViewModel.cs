using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.BuildingBusinessModels
{
    public class BuildingViewModel : BaseEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Floor { get; set; }
        public string? BuildingCode { get; set; }
        public List<RoomInBuilding> Rooms { get; set; } = new List<RoomInBuilding>();
    }

    public class RoomInBuilding
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MaxClassPerTime { get; set; }
        public int AvailabilityeStatus { get; set; }
        public string? RoomCode { get; set; }
        public int RoomType { get; set; }
    }
}
