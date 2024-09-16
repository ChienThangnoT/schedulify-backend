using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ConfigAttributeBusinessModels
{
    public class ConfigAttributeBusinessModel : BaseEntity
    {
        public string? AttributeCode { get; set; }
        public string? Name { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int DefaultValue { get; set; }
        public bool IsRequire { get; set; }

        
        public bool IsWithinRange(int value)
        {
            return value >= MinValue && value <= MaxValue;
        }

        public bool IsRequired()
        {
            return IsRequire;
        }
    }
}
