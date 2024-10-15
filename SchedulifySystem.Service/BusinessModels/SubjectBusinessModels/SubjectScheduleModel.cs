using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectScheduleModel
    {
        public int Id { get; set; }
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public bool IsRequired { get; set; }
    }
}
