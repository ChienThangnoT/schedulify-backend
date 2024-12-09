using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels
{
    public class TeacherAssignmentTermViewModel 
    {
        public int TermId { get; set; }
        public string? TermName { get; set;}
        public List<TeacherAssignmentViewModel> Assignments { get; set; } = [];
        public List<TeacherPeriodCountViewModel> TeacherPeriodsCount { get; set; } = [];
        public List<TeacherAssignmentMinimalData> AssignmentMinimalData { get; set; } = [];
    }
}
