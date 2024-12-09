using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class GetTeacherInSlotModel
    {
        public int TermId { get; set; }
        public int ClassPeriodId { get; set; }
        public DateTime Day { get; set; }
        public int StartAt { get; set; }
    }
}
