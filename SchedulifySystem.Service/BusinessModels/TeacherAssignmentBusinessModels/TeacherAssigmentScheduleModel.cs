using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
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
        public AssignmentType StudentClassId { get; set; }
        public int AssignmentType { get; set; }
        public int PeriodCount { get; set; }
    }
}
