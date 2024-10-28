using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class TeacherUnavailability : BaseEntity
    {
        public int TeacherId { get; set; }
        public int? StartAt { get; set; }
        public int? Priority { get; set; }
        public string? Reason { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
