using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.NotificationBusinessModels
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public ENotificationType Type { get; set; }
        public string? Link { get; set; }
        public string? NotificationURL { get; set; }
    }
}
