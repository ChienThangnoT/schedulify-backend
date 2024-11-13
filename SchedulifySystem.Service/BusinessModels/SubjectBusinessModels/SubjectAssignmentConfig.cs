using SchedulifySystem.Service.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectAssignmentConfig
    {
        public int SubjectId { get; set; }
        public int MainSlotPerWeek { get; set; }
        public int SubSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public bool IsDoublePeriod { get; set; }
        public int MainMinimumCouple { get; set; }
        public int SubMinimumCouple { get; set; }
        public int TermId { get; set; }

        public bool CheckValid()
        {
            return  (IsDoublePeriod && MainMinimumCouple > 0 && MainSlotPerWeek > 0 && MainSlotPerWeek / 2 >= MainMinimumCouple) ||
                    (IsDoublePeriod && SubMinimumCouple > 0 && SubSlotPerWeek > 0 && SubSlotPerWeek / 2 >= SubMinimumCouple) || 
                    (!IsDoublePeriod && (MainSlotPerWeek > 0 || SubSlotPerWeek > 0) && MainMinimumCouple == 0 && SubMinimumCouple == 0);
        }
    }
}
