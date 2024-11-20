using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class ClassCombination
    {
        public int SubjectId { get; set; }
        public List<int> ClassIds { get; set; }
        public int RoomId { get; set; }
        public MainSession Session { get; set; }
        public int? TeacherId { get; set; }
    }
}
