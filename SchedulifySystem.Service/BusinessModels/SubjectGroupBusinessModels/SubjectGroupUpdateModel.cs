using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class SubjectGroupUpdateModel
    {
        public string? GroupName { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupDescription { get; set; }
        public EGrade Grade { get; set; }
        public bool IsDeleted { get; set; }
        public int? SchoolYearId { get; set; }
        public List<int> ElectiveSubjectIds { get; set; } = new List<int>();
        public List<int> SpecializedSubjectIds { get; set; } = new List<int>();
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public int? SchoolId { get; set; }
    }
}
