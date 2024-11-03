using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeachableSubjectBusinessModels
{
    public class TeachableSubjectDetailsViewModel : BaseEntity
    {
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherAbreviation { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectAbreviation { get;set; }
    }
}
