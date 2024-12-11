using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class ClassCombinationViewModel
    {
        public int ClassCombinationId { get; set; }
        public string? ClassCombinationName { get; set; }
        public string? ClassCombinationCode { get; set; }
        public List<StudentClassViewName> Classes { get; set; }
        public List<int> StartAt {  get; set; }
        public int TeacherId { get; set; }
        public string? TeacherAbbreviation { get; set; }
        public string? TeacherName { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set;}
        public string? SubjectAbbreviation { get; set; }
        
    }
}
