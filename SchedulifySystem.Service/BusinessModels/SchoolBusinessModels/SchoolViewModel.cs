using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SchoolBusinessModels
{
    public class SchoolViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public SchoolStatus Status { get; set; }
    }
}
