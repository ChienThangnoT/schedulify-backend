using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class UpdateTimeTableStatusModel
    {
        public required int termId { get; set; }
        public required int startWeek { get; set; }
        public required int endWeek { get; set; }
        public required ScheduleStatus scheduleStatus { get; set; }
    }
}
