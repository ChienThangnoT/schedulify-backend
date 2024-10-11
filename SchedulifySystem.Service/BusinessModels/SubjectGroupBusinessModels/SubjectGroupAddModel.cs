﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class SubjectGroupAddModel
    {
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public string? SubjectGroupTypeCode { get; set; }
        public string? GroupCode { get; set; }
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
