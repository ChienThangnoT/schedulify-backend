using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.PeriodChangeBusinessModels
{
    public class PeriodChangeModel
    {
        public required int ClassPeriodId { get; set; }
        public required int StartAt { get; set; }
        public required int Week { get; set; }
        public int? TeacherId { get; set; }
        public int? RoomId { get; set; }
    }
}
