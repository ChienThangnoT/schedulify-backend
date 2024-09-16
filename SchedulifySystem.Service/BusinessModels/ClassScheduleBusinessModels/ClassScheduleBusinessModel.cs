using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassScheduleBusinessModels
{
    public class ClassScheduleBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? SchoolScheduleName { get; set; }
        public int SchoolScheduleId { get; set; }
    }
}
