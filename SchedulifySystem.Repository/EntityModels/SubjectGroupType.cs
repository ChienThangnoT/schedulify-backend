using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SubjectGroupType : BaseEntity
    {
        public int DefaultSlotPerTerm { get; set; }
        public string? Description { get; set; } 
        public string? Name { get; set; }
        public ICollection<SubjectGroup> subjectGroups { get; set; } = new List<SubjectGroup>();    
    }
}
