using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels
{
    public class CurriculumBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? SchoolName { get; set; }
        public string? SchoolYearName { get; set; }
        public string? ClassGroupName { get; set; }

        
    }
}
