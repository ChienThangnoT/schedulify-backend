using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.NotificationBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToAll(NotificationModel notification);
        Task SendNotificationToUser(int accountId, NotificationModel notification);
    }
}
