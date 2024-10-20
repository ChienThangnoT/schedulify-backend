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
        public int SubjectId { get; set; }
        public string? Abbreviation { get; set; }
        public string? SubjectName { get; set; }
        public int MoringSlotPerWeek { get; set; }
        public int AfternoonSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }

    }
}
