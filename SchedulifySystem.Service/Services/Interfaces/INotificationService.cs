using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.NotificationBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface INotificationService
    {
        Task<BaseResponseModel> GetAllNotificationsByAccountIdAsync(int accountId, bool? isRead);
        Task<BaseResponseModel> GetNumbersOfUnReadNotification(int accountId);
        Task<BaseResponseModel> MakeNotificationsIsReadAsync(int id);
        Task<BaseResponseModel> MakeAllNotificationsIsReadAsync(int accountId);
        Task SendNotificationToAll(NotificationModel notification);
        Task SendNotificationToUser(int accountId, NotificationModel notification);
    }
}
