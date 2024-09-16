using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ClassGroupBusinessModels
{
    public class ClassGroupBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? SchoolYear { get; set; }
        public string? SchoolName { get; set; }
        public int ParentId { get; set; }
        public string? Description { get; set; }
        
        public bool IsParentGroup()
        {
            return ParentId == 0;
        }

       
    }
}
