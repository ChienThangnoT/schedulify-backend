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

        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
