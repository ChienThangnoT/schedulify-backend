using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class AccountDetailModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarURL { get; set; }
        public int? SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<string>? AccountRole {  get; set; }
        public AccountStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
