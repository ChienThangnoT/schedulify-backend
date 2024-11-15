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
    public class CurriculumViewModel
    {
        public int Id { get; set; }
        public string? CurriculumName { get; set; }
        public EGrade Grade { get; set; }

    }

    public class CurriculumViewDetailModel
    {
        public int Id { get; set; }
        public string? CurriculumName { get; set; }
        public EGrade Grade { get; set; }
        public List<SubjectViewDetailModel>? SubjectSelectiveViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectSpecializedtViews { get; set; }
        public List<SubjectViewDetailModel>? SubjectRequiredViews { get; set; }
        public List<string>? StudentClassGroupViewNames { get; set; }

    }
}
