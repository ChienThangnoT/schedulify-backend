using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SubjectGroup : BaseEntity
    {
        public string? GroupName { get; set; }
        public int ClassGroupId { get; set; }
        public ClassGroup? ClassGroup { get; set; }

        public ICollection<SubjectInGroup> SubjectInGroups { get; set; } = new List<SubjectInGroup>();
    }
}
