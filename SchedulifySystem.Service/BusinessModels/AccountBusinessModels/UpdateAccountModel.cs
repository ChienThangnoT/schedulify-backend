using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class UpdateAccountModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? AvatarURL { get; set; }
    }

    public class UpdateStatus
    {
        public required int AccountId { get; set; }
        public required AccountStatus AccountStatus { get; set; }
    }
}
