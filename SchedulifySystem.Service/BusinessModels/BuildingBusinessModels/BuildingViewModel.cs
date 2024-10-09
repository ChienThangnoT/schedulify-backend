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
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Floor { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
