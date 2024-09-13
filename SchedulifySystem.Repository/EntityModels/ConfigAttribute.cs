using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class ConfigAttribute : BaseEntity
    {
        public string? AttributeCode { get; set; }
        public int ConfigGroupId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int IsHardConfig { get; set; }
        public bool IsRequire { get; set; } = false;
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int DefaultValue { get; set; }
        public int Status { get; set; }
        public int DisplayOrder { get; set; }

        public ConfigGroup? ConfigGroup { get; set; }
        public ICollection<TeacherConfig> TeacherConfigs { get; set; } = new List<TeacherConfig>();
        public ICollection<ScheduleConfig>  ScheduleConfigs {  get; set; } = new List<ScheduleConfig>();
        public ICollection<SubjectConfig> SubjectConfigs { get; set; } = new List<SubjectConfig>();
    }

}
