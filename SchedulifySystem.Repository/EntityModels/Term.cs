using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Term : BaseEntity
    {
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SchoolYearId { get; set; }
        public int SchoolId { get; set; }

        public ICollection<SchoolSchedule> SchoolSchedules { get; set; } = new List<SchoolSchedule>();
        public SchoolYear? SchoolYear { get; set; }
        public School? School { get; set; }
        public ICollection<SubjectInGroup> SubjectInGroups { get; set; } = new List<SubjectInGroup>();
    }
}
