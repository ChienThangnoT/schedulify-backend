using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels
{
    public class CurriculumDetailUpdateModel
    {
        public int CurriculumDetailId { get; set; }
        public int MainSlotPerWeek { get; set; }
        public int SubSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public bool IsDoublePeriod { get; set; }
        public int MainMinimumCouple { get; set; }
        public int SubMinimumCouple { get; set; }

    }
}
