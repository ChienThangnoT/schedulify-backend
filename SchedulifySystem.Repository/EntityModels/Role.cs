using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Role : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
    }
}
