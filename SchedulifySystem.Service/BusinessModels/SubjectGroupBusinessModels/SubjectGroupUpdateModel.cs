using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class SubjectGroupUpdateModel
    {
        public string? GroupName { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupDescription { get; set; }
        public Grade Grade { get; set; }
        public bool IsDeleted { get; set; }
        public int? SchoolYearId { get; set; }
    }
}
