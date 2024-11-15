using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels
{
    public class StudentClassGroupViewModel : BaseEntity
    {
        public string? GroupName { get; set; }
        public int? SchoolId { get; set; }
        public int? CurriculumId { get; set; }
        public string? GroupDescription { get; set; }
        public string? StudentClassGroupCode { get; set; }
        public int Grade { get; set; }
        public int? SchoolYearId { get; set; }

    }
}
