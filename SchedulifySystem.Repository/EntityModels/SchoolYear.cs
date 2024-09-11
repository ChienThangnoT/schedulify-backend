using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SchoolYear
    {
        public int SchoolYearId { get; set; }
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }

        public ICollection<SchoolSchedule> SchoolSchedules { get; set; }
        public ICollection<Curriculum> Curriculums { get; set; }
        public ICollection<Term> Terms { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; }
        public ICollection<ClassGroup> ClassGroups { get; set; }
    }
}
