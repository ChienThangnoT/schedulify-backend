using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubmitRequestBusinessModels
{
    public class ProcessSubmitRequestModel
    {
        public required ERequestStatus Status { get; set; }
        public required string? ProcessNote { get; set; }
    }
}
