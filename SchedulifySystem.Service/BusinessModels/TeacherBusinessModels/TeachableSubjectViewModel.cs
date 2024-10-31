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

    }
}
