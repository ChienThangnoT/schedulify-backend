using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SchoolYear : BaseEntity
    {
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }
        public string? SchoolYearCode { get; set; }
        
        public ICollection<SchoolSchedule> SchoolSchedules { get; set; } = new List<SchoolSchedule>();
        public ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
        public ICollection<Term> Terms { get; set; } = new List<Term>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<SubjectGroup> SubjectGroups { get; set; } = new List<SubjectGroup>();
    }
}
