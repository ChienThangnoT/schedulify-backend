using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SubmitRequest : BaseEntity
    {
        public int TeacherId { get; set; }
        public int RequestType { get; set; }
        public DateTime RequestTime { get; set; }
        public int Status { get; set; }
        public string? RequestDescription { get; set; }
        public string? ProcessNote { get; set; }
        public string? AttachedFile { get; set; }
        public int? SchoolYearId { get; set; }

        public Teacher? Teacher { get; set; }
        public SchoolYear? SchoolYear { get; set; }
    }
}
