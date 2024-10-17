using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectScheduleModel : BaseEntity
    {
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public int? SchoolId { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }
    }
}
