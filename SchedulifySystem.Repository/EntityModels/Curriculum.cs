using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public int ClassGroupId { get; set; }

        public School School { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public ClassGroup ClassGroup { get; set;}
        public ICollection<SubjectConfig> SubjectConfigs { get; set; }
    }
}
