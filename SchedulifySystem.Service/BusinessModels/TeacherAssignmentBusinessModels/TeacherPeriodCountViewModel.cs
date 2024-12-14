using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class TeacherPeriodCountViewModel
    {
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherAbbreviation { get; set; }
        public List<TeacherSlotPerWeekViewModel> TotalPeriodsPerWeek { get; set; } = new List<TeacherSlotPerWeekViewModel>();
    }

    public class TeacherSlotPerWeekViewModel
    {
        public string? SubjectName { get; set; }
        public string? SubjectAbbreviation { get; set; }
        public int SubjectId { get; set; }
        public int PeriodCount { get; set; }
    }
}
