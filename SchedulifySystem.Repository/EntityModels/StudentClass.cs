using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class StudentClass : BaseEntity
    {
        public string? Name { get; set; }
        public int HomeroomTeacherId { get; set; }
        public int SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public int MainSession { get; set; }
        public int Status { get; set; }

        public School? School { get; set; }
        public Teacher? Teacher { get; set; }
        public SchoolYear? SchoolYear { get; set; }

        public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
        public ICollection<StudentClassInGroup> StudentClassInGroups { get; set; } = new List<StudentClassInGroup>();
        public ICollection<SubjectConfig> SubjectConfigs { get; set; } = new List<SubjectConfig>();
    }
}
