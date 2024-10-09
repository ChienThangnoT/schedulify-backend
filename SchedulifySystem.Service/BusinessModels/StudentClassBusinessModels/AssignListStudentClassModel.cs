using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class AssignListStudentClassModel : List<AssignStudentClassModel>
    {
       
        public bool HasDuplicateClassId()
        {
            return this.GroupBy(x => x.ClassId).Any(g => g.Count() > 1);
        }

        public bool HasDuplicateTeacherId()
        {
            return this.GroupBy(x => x.TeacherId).Any(g => g.Count() > 1);
        }

        public List<DuplicateAssign> GetDuplicateAssigns()
        {
            var duplicateAssigns = this
           .GroupBy(x => x.ClassId)
           .Where(g => g.Count() > 1)
           .Select(g => new DuplicateAssign
           {
               ClassId = g.Key,
               TeacherId = g.Select(x => x.TeacherId).ToList()
           })
           .ToList();

            return duplicateAssigns;
        }


        public class DuplicateAssign()
        {
            public int ClassId;
            public List<int> TeacherId;
        }
    }
}
