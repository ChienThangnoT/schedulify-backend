using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class RoomViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public ERoomType RoomType { get; set; }
        public int MaxClassPerTime { get; set; }
        public string? RoomCode { get; set; }
        public int BuildingId { get; set; }
        public AvailabilityStatus AvailabilityeStatus { get; set; }
        public List<RoomSubjectViewModel> Subjects { get; set; }
    }
}
