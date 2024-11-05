using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class FixedPeriodScheduleModel
    {
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; } = null;
        public int StartAt { get; set; }
    }
}
