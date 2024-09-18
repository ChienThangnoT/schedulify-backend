using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class DepartmentBusinessModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? SchoolName { get; set; }
        public int SchoolId { get; set; }

    }
}
