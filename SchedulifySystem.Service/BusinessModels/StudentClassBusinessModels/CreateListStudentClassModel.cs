using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class CreateListStudentClassModel
    {
        public string? Name { get; set; }
        public string? HomeroomTeacherAbbreviation { get; set; }
        public int? MainSession { get; set; }
        public bool IsFullDay { get; set; } = false;
        public int PeriodCount { get; set; }
        public EGrade Grade { get; set; }
        public string? SubjectGroupCode { get; set; }

        [JsonIgnore]
        public int? HomeroomTeacherId { get; set; }
        [JsonIgnore]
        public int? SchoolId { get; set; }
        [JsonIgnore]
        public int? SchoolYearId { get; set; }
        [JsonIgnore]
        public int? SGroupId { get; set; } = null;
    }
}
