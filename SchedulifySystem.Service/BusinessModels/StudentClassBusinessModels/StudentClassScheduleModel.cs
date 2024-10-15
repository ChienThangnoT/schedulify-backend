using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class ClassScheduleModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MainSession { get; set; }
    }
}
