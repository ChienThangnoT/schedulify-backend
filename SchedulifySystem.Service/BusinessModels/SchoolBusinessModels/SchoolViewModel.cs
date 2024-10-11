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
        public int ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public int DistrictCode { get; set; }
        public SchoolStatus Status { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
