using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class ConfigGroup
    {
   
        public int ConfigGroupId { get; set; }
        public string? Name { get; set; }
        public string? GroupType { get; set; }
        public int Status { get; set; }

        public ICollection<ConfigAttribute> ConfigAttributes { get; set; }
    }
}
