using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class ViewAssignmentDetail
    {
        public int TeacherId { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherLastName { get; set; }
        public int TotalSlotInYear { get; set; }
        public int OveragePeriods { get; set; }
        public List<AssignmentTeacherDetail>? AssignmentDetails { get; set; }
    }

    public class AssignmentTeacherDetail
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? ClassName { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public int TotalPeriod { get; set; }
    }
}
