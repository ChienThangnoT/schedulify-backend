using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class DepartmentUpdateModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DepartmentCode { get; set; }
        public int MeetingDay { get; set; }
    }
}
