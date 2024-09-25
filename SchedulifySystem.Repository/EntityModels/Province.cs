using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class Province : BaseEntity
    {
        public string? Name { get; set; }

        public ICollection<EducationDepartment> EducationDepartments { get; set; } = new List<EducationDepartment>();
    }
}
