using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class CheckPeriodChangeModel
    {
        public required int ClassId { get; set; }
        public required List<int> FromStartAts { get; set; }
        public required List<int> ToStartAts { get; set; }
        public required SchoolScheduleDetailsViewModel TimeTableData { get; set; }
    }
}
