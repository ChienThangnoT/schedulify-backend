using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class TeachableSubject
    {
        public int TeachableSubjectId { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }

        public Teacher Teacher { get; set; }
        public Subject Subject { get; set; }
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; }
    }
}
