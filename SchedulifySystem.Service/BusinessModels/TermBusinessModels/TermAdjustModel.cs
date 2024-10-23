using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TermBusinessModels
{
    public class TermAdjustModel
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SchoolYearId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
