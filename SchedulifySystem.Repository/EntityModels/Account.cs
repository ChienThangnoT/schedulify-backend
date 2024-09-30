using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Account : BaseEntity
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public int SchoolId { get; set; }
        public string? Email { get; set; }
        public bool IsChangeDefaultPassword { get; set; }
        public int Status { get; set; }
        public string? Phone {  get; set; }
        public string? AvatarURL { get; set; }

        public School? School { get; set; }
        public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
