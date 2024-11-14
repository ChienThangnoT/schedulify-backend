using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class CurriculumDetail : BaseEntity
    {
        public int SubjectId { get; set; }
        public int MainSlotPerWeek { get; set; }
        public int SubSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int? TermId { get; set; }
        public int? CurriculumId { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }
        public int SubjectInGroupType {  get; set; }
        public int MainMinimumCouple { get; set; }
        public int SubMinimumCouple { get; set; }

        public Subject? Subject { get; set; }
        public Term? Term { get; set; }
        public Curriculum? Curriculum { get; set; }

        public CurriculumDetail ShallowCopy()
        {
            return (CurriculumDetail) this.MemberwiseClone();
        }
    }
}
