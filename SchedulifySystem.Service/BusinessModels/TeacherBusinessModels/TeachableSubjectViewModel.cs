using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeachableSubjectViewModel
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public List<ListApproriateLevelByGrade> ListApproriateLevelByGrades { get; set; }
    }

    public class ListApproriateLevelByGrade
    {
        public int Id { get; set; }
        public bool IsMain { get; set; }
        public EAppropriateLevel AppropriateLevel { get; set; }
        public EGrade Grade { get; set; }
    }
}
