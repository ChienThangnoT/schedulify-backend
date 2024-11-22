using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class StudentClassViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public int? HomeroomTeacherId { get; set; }
        public string? HomeroomTeacherName { get; set; }
        public string? HomeroomTeacherAbbreviation { get; set; }
        public int MainSession { get; set; }
        public string? MainSessionText { get; set; }
        public EGrade Grade { get; set; }
        public bool IsFullDay { get; set; }
        public int PeriodCount { get; set; }
        public int? StudentClassGroupId { get; set; }
        public string? StudentClassGroupName { get; set; }
        public string? StudentClassGroupCode { get; set; }
        public int? CurriculumId { get; set; }
        public string? CurriculumName { get; set; }
        public int? SchoolYearId { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
    }

    public class StudentClassViewName
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
