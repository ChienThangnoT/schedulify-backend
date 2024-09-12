using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ClassGroup : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int SchoolYearId { get; set; }
        public int ParentId { get; set; }
        public int SchoolId { get; set; }


        public School? School { get; set; }
        public SchoolYear? SchoolYear { get; set; }
        public SubjectGroup? SubjectGroup { get; set; }
        public ICollection<Curriculum> CurriculumList { get; set; } = new List<Curriculum>();
        public ICollection<StudentClassInGroup> StudentClassInGroups { get; set; } = new List<StudentClassInGroup>();


    }
}
