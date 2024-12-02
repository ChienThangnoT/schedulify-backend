using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassScheduleBusinessModels
{
    public class ClassScheduleViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public int SchoolScheduleId { get; set; }
        public int? StudentClassId { get; set; }
        public string? StudentClassName { get; set; }

        public List<ClassPeriodViewModel>? ClassPeriods { get; set; }

    }
}
