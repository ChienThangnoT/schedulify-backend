using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SubjectGroup
    {
        public int SubjectGroupId { get; set; }
        public string GroupName { get; set; }
        public int ClassGroupId { get; set; }

        public ICollection<SubjectInGroup> SubjectInGroups { get; set; }
        public ClassGroup ClassGroup { get; set; }
    }
}
