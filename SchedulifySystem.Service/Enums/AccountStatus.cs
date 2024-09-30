using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum AccountStatus
    {
        Active = 1,
        Pending = 2,
        Inactive = 3,
    }
}
