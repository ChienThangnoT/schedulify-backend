using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class ClassGroup
    {
        public int ClassGroupId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int SchoolYearId { get; set; }
        public int ParentId { get; set; }
        public int SchoolId { get; set; }


        public School School { get; set;}
        public ICollection<Curriculum> CurriculumList { get; set; }
        public ICollection<StudentClassInGroup> StudentClassInGroups { get; set;}
        public SchoolYear SchoolYear { get; set; }
        public SubjectGroup SubjectGroup { get; set; }
      
        
    }
}
