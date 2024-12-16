using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class ClassCombination
    {
        public int SubjectId { get; set; }
        public List<StudentClassViewName> Classes { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public string? RoomSubjectCode { get; set; }
        public string? RoomSubjectName { get; set; }
        public MainSession Session { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        [JsonIgnore]
        public int Id { get; set; }
    }
}
