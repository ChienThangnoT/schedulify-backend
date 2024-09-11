using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class TeacherUnavailability
    {
        public int TeacherUnavailabilityId { get; set; } 
        public int TeacherId { get; set; }
        public int DateOfWeek { get; set; }
        public int WeekNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Teacher Teacher { get; set; }
    }
}
