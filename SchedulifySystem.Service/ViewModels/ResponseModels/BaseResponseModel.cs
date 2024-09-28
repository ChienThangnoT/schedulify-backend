using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.ViewModels.ResponseModels
{
    public class BaseResponseModel
    {
        public required int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Result { get; set; } = new object();
    }
}
