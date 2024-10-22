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
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int? ClassScheduleId { get; set; }
        [JsonIgnore]
        public int? TimeSlotId { get; set; }
        [JsonIgnore]
        public int? RoomId { get; set; }
        [JsonIgnore]
        public int? Status { get; set; }
        [JsonIgnore]
        public int? TeacherId { get; set; }
        public int? SubjectId { get; set; }
        [JsonIgnore]
        public int? DateOfWeek { get; set; }
        public int? ClassId { get; set; }
        [JsonIgnore]
        public string? ClassName { get; set; }
        [JsonIgnore]
        public string? SubjectAbbreviation { get; set; }
        [JsonIgnore]
        public string? TeacherAbbreviation { get; set; }
        [JsonIgnore]
        public string? RoomCode { get; set; }
        [JsonIgnore]
        public int? TeacherAssignmentId { get; set; }
        
        public int StartAt { get; set; }
        [JsonIgnore]
        public EPriority Priority { get; set; } = EPriority.None;
        [JsonIgnore]
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
