using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassScheduleBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class SchoolScheduleDetailsViewModel : BaseEntity
    {
        public int SchoolYearId { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public int SchoolId { get; set; }
        public int TermId { get; set; }
        public string? TermName { get; set; }
        public string? Name { get; set; }
        public int FitnessPoint { get; set; }
        public double ExcuteTime { get; set; }
        public List<ClassCombinationViewModel> ClassCombinations { get; set; }
        public List<ClassScheduleViewModel> ClassSchedules { get; set; }
    }
}
