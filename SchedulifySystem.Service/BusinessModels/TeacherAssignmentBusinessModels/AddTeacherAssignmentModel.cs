using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class AddTeacherAssignmentModel
    {
        public int TeachableSubjectId {  get; set; }
        public int PeriodCount { get; set; }
        public int TermId { get; set; }
        public int StudentClassId { get; set; }

        [JsonIgnore]
        public AssignmentType AssignmentType { get; set; } = AssignmentType.Permanent;
    }
}
