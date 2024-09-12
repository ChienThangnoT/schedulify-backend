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

        public virtual ICollection<Account> Accounts { get; set; } = [];
    }
}
