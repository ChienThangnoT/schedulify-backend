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
        public int AssignmentType { get; set; }
        public int PeriodCount { get; set; } // số lượng tiết trên tuần 
        public int TermId { get; set; }

        public Term? Term { get; set; }
        public TeachableSubject? TeachableSubject { get; set; }
        public StudentClass? StudentClass { get; set; }
        public ICollection<ClassPeriod> ClassPeriods { get; set; } = new List<ClassPeriod>();
    }
}
