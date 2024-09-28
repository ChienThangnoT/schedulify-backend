using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class AccountBusinessModel : BaseEntity
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public RoleEnum RoleType { get; set; }
        public string? SchoolName { get; set; }
    }
}
