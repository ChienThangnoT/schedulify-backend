using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectScheduleModel : BaseEntity
    {
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
        public int? SchoolId { get; set; }
        public int? TotalSlotInYear { get; set; }
        public int? SlotSpecialized { get; set; }

        public SubjectScheduleModel(Subject subject)
        {
            Id = subject.Id;
            SubjectName = subject.SubjectName;
            Abbreviation = subject.Abbreviation;
            Description = subject.Description;
            IsRequired = subject.IsRequired;
            SchoolId = subject.SchoolId;
            TotalSlotInYear = subject.TotalSlotInYear;
            SlotSpecialized = subject.SlotSpecialized;
        }
    }
}
