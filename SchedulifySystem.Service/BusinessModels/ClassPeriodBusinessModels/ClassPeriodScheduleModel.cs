using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels
{
    public record ClassPeriodScheduleModel 
    {
        private static int count = 0;
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
        public string? SubjectName { get; set; }

        public string? TeacherAbbreviation { get; set; }
        
        public string? RoomCode { get; set; }
        
        public int? TeacherAssignmentId { get; set; }
        
        public int StartAt { get; set; }
        
        public EPriority Priority { get; set; } = EPriority.None;
        public MainSession Session { get; set; }
        public ClassCombination? ClassCombinations { get; set; }
        public int periodsTh { get; set; }
        public List<ConstraintErrorModel> ConstraintErrors { get; set; } = [];

        public ClassPeriodScheduleModel()
        {
            Id = ++count;
        }

        public ClassPeriodScheduleModel(TeacherAssigmentScheduleModel assignment)
        {
            Id = ++count;
            SubjectId = assignment.Subject.SubjectId;
            SubjectAbbreviation = assignment.Subject.Abbreviation;
            SubjectName = assignment.Subject.SubjectName;
            TeacherId = assignment.Teacher.Id;
            TeacherAbbreviation = assignment.Teacher.Abbreviation;
            ClassName = assignment.StudentClass.Name;
            ClassId = assignment.StudentClass.Id;
            TeacherAssignmentId = assignment.Id;
        }

    }
}
