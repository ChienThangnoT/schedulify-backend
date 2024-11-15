using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels
{
    public class CurriculumDetailViewModel
    {
        public int Id { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public int SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public string? GroupDescription { get; set; }
        public EGrade Grade { get; set; }
        public string? SubjectGroupTypeName { get; set; }
        public int? SchoolYearId { get; set; }

    }

    public class CurriculumViewDetailModel
    {
        public int Id { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public int SchoolId { get; set; }
        public string? GroupDescription { get; set; }
        public EGrade Grade { get; set; }
        public List<SubjectViewDetailModel>? SubjectSelectiveViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectSpecializedtViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectRequiredViews { get; set; }
        public List<StudentClassGroupViewName>? StudentClassGroupViewNames { get; set; }
        public string? SubjectGroupTypeName { get; set; }
        public int? SchoolYearId { get; set; }
        public string? SchoolYear { get; set; }

    }
}
