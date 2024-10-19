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
        public int SlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int TermId { get; set; }
        public bool IsSpecialized { get; set; }
    }
}
