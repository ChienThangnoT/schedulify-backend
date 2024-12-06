using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class AssignTeacherAssignmentModel
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
    }
}
