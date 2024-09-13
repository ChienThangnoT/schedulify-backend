using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Holiday : BaseEntity
    {
        public int SchoolId { get; set; }
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int HolidayType { get; set; }

        public School? School { get; set; }
    }
}