﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IClaimsService
    {
        public string? GetCurrentUserEmail { get; }
        public string? GetCurrentSchoolId { get; }
    }
}
