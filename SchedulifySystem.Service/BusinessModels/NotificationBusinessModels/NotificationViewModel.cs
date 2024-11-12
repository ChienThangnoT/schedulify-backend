using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.NotificationBusinessModels
{
    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public ENotificationType Type { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public DateTime ReadAt { get; set; }
        public string? Link { get; set; }
        public string? NotificationURL { get; set; }
    }
}
