using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Enums
{
    public enum ScheduleStatus
    {
        Draft = 1,
        Published = 2,
        PublishedInternal = 3,
        Expired = 4,
        Disabled = 5
    }
}
