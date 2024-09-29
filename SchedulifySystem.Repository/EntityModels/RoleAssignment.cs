using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class RoleAssignment : BaseEntity
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public int? DepartmentId { get; set; }
        
        public Account? Account { get; set; }
        public Role? Role { get; set; }
        public Department? Department { get; set; }

    }
}
