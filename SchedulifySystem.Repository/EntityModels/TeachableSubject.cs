using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class TeachableSubject : BaseEntity
    {
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public int AppropriateLevel { get; set; }
        public int Grade { get; set; }
        public bool IsMain {  get; set; }

        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
        
    }
}
