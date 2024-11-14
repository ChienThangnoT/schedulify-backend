using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class Curriculum : BaseEntity
    {
        public string? CurriculumName { get; set; }
        public int SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public int Grade { get; set; }

        public SchoolYear? SchoolYear { get; set; }
        public School? School { get; set; }
        public ICollection<CurriculumDetail> CurriculumnDetails { get; set; } = new List<CurriculumDetail>();

    }
}
