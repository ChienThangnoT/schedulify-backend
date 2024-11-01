using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SubjectGroup : BaseEntity
    {
        public string? GroupName { get; set; }
        public int? SchoolId { get; set; }
        public string? GroupDescription { get; set; }
        public string? GroupCode { get; set; }
        public int Grade {  get; set; }
        public int? SchoolYearId { get; set; }


        public School? School { get; set; }
        public SchoolYear? SchoolYear { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<SubjectInGroup> SubjectInGroups { get; set; } = new List<SubjectInGroup>();
        public ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
    }
}
