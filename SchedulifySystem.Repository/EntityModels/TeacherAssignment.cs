using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class TeacherAssignment : BaseEntity
    {
        public int TeachableSubjectId { get; set; }
        public int StudentClassId { get; set; }
        public TeachableSubject? TeachableSubject { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}
