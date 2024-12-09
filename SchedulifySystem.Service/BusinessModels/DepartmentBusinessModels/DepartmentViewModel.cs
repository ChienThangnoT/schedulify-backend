using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class DepartmentViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DepartmentCode { get; set; }
        public int? MeetingDay { get; set; }
        public int? TeacherDepartmentHeadId { get; set; }
        public string? TeacherDepartmentFirstName { get; set; }
        public string? TeacherDepartmentLastName { get; set; }
        public string? TeacherDepartmentAbbreviation { get; set; }

    }
}
