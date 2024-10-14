using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class RoomBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public int MaxClassPerTime { get; set; }
        public string? BuildingName { get; set; }
        public string? RoomTypeName { get; set; }
        public string? RoomCode { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public bool IsAvailable()
        {
            return AvailabilityStatus == AvailabilityStatus.Available;  
        }

        public bool IsMultipleClassCanLearn()
        {
            return MaxClassPerTime > 1;
        }

        public bool CanAccommodate(int numberOfClasses)
        {
            return numberOfClasses <= MaxClassPerTime;
        }
    }
}
