using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class ScheduleConfig
    {
        public int ScheduleConfigId { get; set; }
        public int ConfigAttributeId { get; set; }
        public int SchoolScheduleId { get; set; }
        public int Value { get; set; }

        public ConfigAttribute ConfigAttribute { get; set; }
        public SchoolSchedule SchoolSchedule { get; set; }

    }
}
