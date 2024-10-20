using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels
{
    public record ClassPeriodScheduleModel 
    {
        public int Id { get; set; }
        public int? ClassScheduleId { get; set; }
        public int? TimeSlotId { get; set; }
        public int? RoomId { get; set; }
        public int? Status { get; set; }
        public int? TeacherId { get; set; }
        public int? SubjectId { get; set; }
        public int? DateOfWeek { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectAbbreviation { get; set; }
        public string? TeacherAbbreviation { get; set; }
        public string? RoomCode { get; set; }
        public int? TeacherAssignmentId { get; set; }
        
        public int StartAt { get; set; }
        public int Priority { get; set; }

        public List<ConstraintErrorModel> ConstraintErrors { get; set; } = [];

        public ClassPeriodScheduleModel()
        {
            
        }

        public ClassPeriodScheduleModel(TeacherAssigmentScheduleModel assignment)
        {
            SubjectId = assignment.Subject.SubjectId;
            SubjectAbbreviation = assignment.Subject.Abbreviation;
            TeacherId = assignment.Teacher.Id;
            TeacherAbbreviation = assignment.Teacher.Abbreviation;
            ClassName = assignment.StudentClass.Name;
            ClassId = assignment.StudentClass.Id;
            TeacherAssignmentId = assignment.Id;
        }

    }
}
