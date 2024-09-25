using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class EducationDepartment : BaseEntity
    {
        public int ProvinceId { get; set; }
        public string? Name { get; set; }

        public Province? Province { get; set; }
        public ICollection<School> Schools { get; set; } = new List<School>();
    }
}
