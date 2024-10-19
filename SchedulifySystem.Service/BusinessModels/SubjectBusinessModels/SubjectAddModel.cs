using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectAddModel
    {
        public int SchoolId { get; set; }
        public required string SubjectName { get; set; }
        public required string Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public required string Description { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }
    }
    public class SubjectAddListModel
    {
        public required string SubjectName { get; set; }
        public required string Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public required string Description { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; } = 35;
    }
}
