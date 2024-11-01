using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels
{
    partial class SchoolYearViewModel
    {
        public int Id { get; set; }
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }
        public string? SchoolYearCode { get; set; }
    }
}
