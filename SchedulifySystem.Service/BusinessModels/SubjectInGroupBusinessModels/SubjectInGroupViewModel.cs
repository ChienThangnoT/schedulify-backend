using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels
{
    public class SubjectInGroupViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectGroupId { get; set; }
        public int MoringSlotPerWeek { get; set; }
        public int AfternoonSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int TermId { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }
    }

    public class SubjectInGroupViewDetailModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int SubjectGroupId { get; set; }
        public int MoringSlotPerWeek { get; set; }
        public int AfternoonSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int TermId { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }
        public List<SubjectViewDetailModel>? subjectSelectiveViews { get; set; }
        public List<SubjectViewDetailModel>? subjecSpecializedtViews { get; set; }
    }
}
