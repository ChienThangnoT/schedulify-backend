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
        public int Id { get; set; }
        public int TotalSlotInYear { get; set; }
        public bool IsMainSessionStudy { get; set; }
        public bool IsSubSessionStydy { get; set; }
        public int? MainSlotPerWeek {  get; set; } 

        public bool CheckValid()
        {
            return  (IsMainSessionStudy && !IsSubSessionStydy) || 
                    (!IsMainSessionStudy && IsSubSessionStydy) || 
                    (IsMainSessionStudy && IsSubSessionStydy && MainSlotPerWeek != null);
        }
    }
}
