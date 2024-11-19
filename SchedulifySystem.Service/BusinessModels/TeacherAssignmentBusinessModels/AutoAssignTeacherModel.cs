using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class AutoAssignTeacherModel
    {
        public List<FixedTeacherAssignmentModel>? fixedAssignment {  get; set; } = new List<FixedTeacherAssignmentModel>();
        public List<ClassCombination>? classCombinations { get; set; }
    }
}
