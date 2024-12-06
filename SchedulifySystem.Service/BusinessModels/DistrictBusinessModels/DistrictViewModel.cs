using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DistrictBusinessModels
{
    public class DistrictViewModel
    {
        public int ProvinceId { get; set; }
        public string? Name { get; set; }
        public int? DistrictCode { get; set; }
    }
}
