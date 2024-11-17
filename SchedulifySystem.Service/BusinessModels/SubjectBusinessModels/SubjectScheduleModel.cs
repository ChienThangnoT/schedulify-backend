using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public record SubjectScheduleModel 
    {
        public int SubjectId { get; set; }
        public string? Abbreviation { get; set; }
        public string? SubjectName { get; set; }
        public int MainSlotPerWeek { get; set; }
        public int SubSlotPerWeek { get; set; }
        public int SlotPerTerm { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }
        public bool IsSpecialized { get; set; }
        public bool IsDoublePeriod { get; set; }
        public int SubjectGroupId { get; set; }
        public int MainMinimumCouple { get; set; }
        public int SubMinimumCouple { get; set; }


        public SubjectScheduleModel() { }

        public SubjectScheduleModel(CurriculumDetail sig)
        {
           SubjectId = sig.SubjectId;
            Abbreviation = sig.Subject.Abbreviation;
            SubjectName = sig.Subject.SubjectName;
            MainSlotPerWeek = sig.MainSlotPerWeek;
            SubSlotPerWeek = sig.SubSlotPerWeek;
            TotalSlotInYear = sig.Subject.TotalSlotInYear;
            SlotSpecialized = sig.Subject.SlotSpecialized;
            IsSpecialized = sig.IsSpecialized;
            IsDoublePeriod = sig.IsDoublePeriod;
            IsDoublePeriod = sig.IsDoublePeriod;
        }

    }
}
