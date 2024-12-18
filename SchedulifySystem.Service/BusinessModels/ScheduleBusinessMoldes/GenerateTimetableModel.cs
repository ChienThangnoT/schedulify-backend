﻿
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
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
        public int SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public string? TimetableName { get; set; }

        public List<FixedPeriodScheduleModel> FixedPeriodsPara { get; set; }
        public List<NoAssignPeriodScheduleModel> NoAssignPeriodsPara { get; set; }
        public List<FreeTimetablePeriodScheduleModel> FreeTimetablePeriodsPara { get; set; }

        [JsonIgnore]
        public List<ClassPeriodScheduleModel> FixedPeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết cố định 
        [JsonIgnore]
        public List<ClassPeriodScheduleModel> NoAssignTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); //ds tiết không xếp 
        [JsonIgnore]
        public List<ClassPeriodScheduleModel> BusyTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết bận của gv 
        [JsonIgnore] 
        public List<ClassPeriodScheduleModel> FreeTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // Ds tiết trống - dùng cho kiểm tra tiết lủng // bỏ
        [JsonIgnore]
        public List<RoomSubjectScheduleModel> PracticeRoomWithSubjects { get; set; } = new List<RoomSubjectScheduleModel>(); //

        public int MaxPeriodPerSession { get; set; } = 5;
        public int MinPeriodPerSession { get; set; } = 0;
        public int TermId { get; set; }
    }
}
