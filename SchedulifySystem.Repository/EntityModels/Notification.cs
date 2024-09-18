using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Notification : BaseEntity
    {
        public int AccountId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public int Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime ReadAt { get; set; }
        public string? Link { get; set; }
        public string? NotificationURL { get; set; }

        public Account? Account { get; set; }
    }
}
