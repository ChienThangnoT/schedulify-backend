
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class GenerateTimetableModel
    {
        [JsonIgnore]
        public string? CurrentUserEmail { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        public int SchoolYearId { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string? TimetableName { get; set; }

        public List<FixedPeriodScheduleModel>? FixedPeriodsPara { get; set; }
        public List<NoAssignPeriodScheduleModel>? NoAssignPeriodsPara { get; set; }
        public List<FreeTimetablePeriodScheduleModel>? FreeTimetablePeriodsPara { get; set; }
        public required List<TeacherAssignmentMinimalData> TeacherAssignments { get; set; }

        [JsonIgnore]
        public List<ClassPeriodScheduleModel> FixedPeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết cố định 
        [JsonIgnore]
        public List<ClassPeriodScheduleModel> NoAssignTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); //ds tiết không xếp 
        [JsonIgnore]
        public List<ClassPeriodScheduleModel> BusyTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết bận của gv 
        [JsonIgnore]
        public List<ClassPeriodScheduleModel> FreeTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // Ds tiết trống - dùng cho kiểm tra tiết lủng // bỏ
        [JsonIgnore]
        public List<RoomSubjectScheduleModel> PracticeRooms { get; set; } = new List<RoomSubjectScheduleModel>(); //
        
        [JsonIgnore]
        public List<ClassCombination>? ClassCombinations { get; set; } = new List<ClassCombination>(); //

        public int RequiredBreakPeriods { get; set; } = 1;
        public int MinimumDaysOff { get; set; } = 0;
        public int TermId { get; set; }
        public int DaysInWeek { get; set; } = 5;
        public int MaxExecutionTimeInSeconds { get; set; } = 600;

        public int GetAvailableSlotsPerWeek()
        {
            return (DaysInWeek + 1) * 10 + 1;
        }
    }
}
