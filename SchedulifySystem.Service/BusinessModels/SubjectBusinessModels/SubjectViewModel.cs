using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public int SchoolYearId { get; set; }
        public string? SchoolYearCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int SlotSpecialized { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public ESubjectGroupType SubjectGroupType { get; set; }
    }

    public class SubjectViewDetailModel
    {
        public int Id { get; set; }
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public ESubjectInGroupType? SubjectInGroupType { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public int MainSlotPerWeek { get; set; }
        public int SubSlotPerWeek { get; set; }
        public int TotalSlotPerWeek { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }
        public int SlotPerTerm { get; set; }
        public int TermId { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int SlotSpecialized { get; set; } = 35;
        public ESubjectGroupType SubjectGroupType { get; set; }
        public int MainMinimumCouple { get; set; }
        public int SubMinimumCouple { get; set; }
    }
}
