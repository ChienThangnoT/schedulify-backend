using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TermBusinessModels
{
    public class TermViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SchoolYearId { get; set; }
        public string? SchoolYearCode { get; set; }
        public string? SchoolYearStart { get; set; }
        public string? SchoolYearEnd { get; set; }
    }
}
