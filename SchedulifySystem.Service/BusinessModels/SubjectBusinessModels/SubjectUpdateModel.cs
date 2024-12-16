using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectUpdateModel
    {
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public bool? IsRequired { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }
        public ESubjectGroupType SubjectGroupType { get; set; }
        [JsonIgnore]
        public DateTime UpdateDate { get; set; }
    }
}
