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
        public List<TermViewModel>? TermViewModel { get; set; }
        public bool IsPublic { get; set; }
    }
    public class TermViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
