using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class TeacherAssigmentScheduleModel : BaseEntity
    {
        public int TeachableSubjectId { get; set; }
        public int StudentClassId { get; set; }
        public int AssignmentType { get; set; }
    }
}
