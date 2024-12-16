using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubmitRequest
{
    public class SubmitSendRequestModel
    {
        public required int TeacherId { get; set; }
        public required int SchoolYearId { get; set; }
        public required ERequestType RequestType { get; set; }
        [JsonIgnore]
        public ERequestStatus Status { get; set; }
        public required string RequestDescription { get; set; }
        public string? AttachedFile { get; set; }
    }
}
