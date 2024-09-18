using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.BuildingBusinessModels
{
    public class BuildingBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int Floor { get; set; }
        public string? SchoolName { get; set; }
    }
}
