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
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int SchoolId { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public string? AccessToken { get; set; }

        public School? School { get; set; }  
        public Role? Role { get; set; }  
    }

}
