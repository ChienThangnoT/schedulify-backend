using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class AssignSubjectGroup
    {
        public List<int> ClassIds { get; set; } = new List<int>();
        public int SubjectGroupId { get; set; }
    }
}
