using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectAddModel
    {
        public required string SubjectName { get; set; }
        public required string Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public required string Description { get; set; }
        [JsonIgnore]
        public int? TotalSlotInYear { get; set; }
        [JsonIgnore]
        public int? SlotSpecialized { get; set; } = 35;
        public ESubjectGroupType SubjectGroupType { get; set; }
    }
    public class SubjectAddListModel
    {
        public required string SubjectName { get; set; }
        public required string Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public required string Description { get; set; }
        public int TotalSlotInYear { get; set; }
        [DefaultValue(35)]
        public int SlotSpecialized { get; set; }
        public ESubjectGroupType SubjectGroupType { get; set; }

    }
}
