using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
