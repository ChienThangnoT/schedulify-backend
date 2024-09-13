using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ConfigGroup : BaseEntity
    {
        public string? Name { get; set; }
        public int GroupType { get; set; }
        public int Status { get; set; }

        public ICollection<ConfigAttribute> ConfigAttributes { get; set; } = new List<ConfigAttribute>();
    }
}
