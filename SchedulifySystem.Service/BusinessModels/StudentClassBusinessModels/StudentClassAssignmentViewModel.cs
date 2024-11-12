using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class StudentClassAssignmentViewModel
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public List<AssignmentDetail>? AssignmentDetails { get; set; }
        public int TotalSlotInYear { get; set; }
    }

    public class AssignmentDetail
    {
        public int TermId { get; set; }
        public string? TermName {  get; set; }
        public int TeacherId { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherLastName { get; set; }
        public int TotalPeriod { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
    }

}
