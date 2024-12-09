using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class PeriodChange : BaseEntity
    {
        public int ClassPeriodId { get; set; }
        public int StartAt { get; set; }
        public int? Week {  get; set; }
        public int? TeacherId {  get; set; }
        public int? RoomId {  get; set; }

        public ClassPeriod? ClassPeriod { get; set; }
    }
}
