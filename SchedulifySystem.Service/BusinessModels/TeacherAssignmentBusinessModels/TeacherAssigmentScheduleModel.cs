using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
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
        public TeacherScheduleModel Teacher { get; set; }
        public SubjectScheduleModel Subject { get; set; }
        public ClassScheduleModel StudentClass { get; set; }
        public AssignmentType AssignmentType { get; set; }
        public int PeriodCount { get; set; }
        public int TermId { get; set; }

        public TeacherAssigmentScheduleModel(
            TeacherAssignment teacherAssignment, 
            TeacherScheduleModel teacher, 
            SubjectScheduleModel subject, 
            ClassScheduleModel studentClass)
        {
            Id = teacherAssignment.Id;
            AssignmentType = (AssignmentType)teacherAssignment.AssignmentType;
            PeriodCount = teacherAssignment.PeriodCount;
            TermId = teacherAssignment.TermId;
            Teacher = teacher;
            Subject = subject;
            StudentClass = studentClass;
        }
    }
}
