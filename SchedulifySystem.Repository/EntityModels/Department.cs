using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Department : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int SchoolId { get; set; }
        public int? MeetingDay { get; set; }
        public string? DepartmentCode {  get; set; }
        public School? School { get; set; }
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();

    }

}
