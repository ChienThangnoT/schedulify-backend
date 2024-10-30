using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class SubjectGroupViewModel
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

    public class SubjectGroupViewDetailModel
    {
        public int Id { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public int SchoolId { get; set; }
        public string? GroupDescription { get; set; }
        public EGrade Grade { get; set; }
        public List<SubjectViewDetailModel> ? SubjectSelectiveViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectSpecializedtViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectRequiredViews { get; set; }
        public List<StudentClassViewName>? StudentClassViews { get; set; }
        public string? SubjectGroupTypeName { get; set; }
        public int? SchoolYearId { get; set; }
        public string? SchoolYear { get; set; }

    }
}
