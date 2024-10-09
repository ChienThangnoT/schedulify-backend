using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.ViewModels.ResponseModels
{
    public class AuthenticationResponseModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string JwtToken { get; set; }
        [Ignore]
        [IgnoreDataMember]
        [JsonIgnore]
        public bool IschangePasswordDefault { get; set; }
        public DateTime? Expired { get; set; }
        public string JwtRefreshToken { get; set; }
    }
}
