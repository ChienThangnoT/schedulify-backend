using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class GenerateTimetableModel
    {
        public int SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public List<ClassPeriodScheduleModel> FixedPeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết cố định 
        public List<ClassPeriodScheduleModel> NoAssignTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); //ds tiết không xếp 
        public List<ClassPeriodScheduleModel> BusyTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // ds tiết bận của gv 
        public List<ClassPeriodScheduleModel> FreeTimetablePeriods { get; set; } = new List<ClassPeriodScheduleModel>(); // Ds tiết trống - dùng cho kiểm tra tiết lủng // bỏ

        public int MaxPeriodPerSession { get; set; } = 5;
        public int MinPeriodPerSession { get; set; } = 0;
        public int? TermId { get; set; }
    }
}
