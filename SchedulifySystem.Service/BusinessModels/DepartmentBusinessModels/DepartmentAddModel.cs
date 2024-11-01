using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class DepartmentAddModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DepartmentCode { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
    }
}
