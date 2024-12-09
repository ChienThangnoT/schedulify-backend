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
        public int TotalPeriodsPerWeek { get; set; }
    }
}
