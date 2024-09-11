using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class SubjectInGroup
    {
        public int SubjectInGroupId { get; set; }
        public int SubjectId { get; set; }
        public int SubjectGroupId { get; set; }

        public Subject Subject { get; set; }
        public SubjectGroup SubjectGroup { get; set; }
    }
}
