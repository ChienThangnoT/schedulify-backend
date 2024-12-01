using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.NotificationBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Hubs;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Get All Notifications By Account Id
        public async Task<BaseResponseModel> GetAllNotificationsByAccountIdAsync(int accountId, bool? isRead, int pageIndex, int pageSize)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(accountId, filter: t => t.Status == (int)AccountStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);

            var notification = await _unitOfWork.NotificationRepo.GetPaginationAsync(
                filter: t => t.AccountId == accountId && (isRead == null || t.IsRead == isRead) && t.IsDeleted == false,
                orderBy: query => query.OrderByDescending(t => t.CreateDate),
                pageIndex: pageIndex,
                pageSize: pageSize);

            if (notification == null || notification.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.GET_NOTIFICATION_NOT_EXIST);
            }

            var result = _mapper.Map<Pagination<NotificationViewModel>>(notification);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_NOTIFICATION_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region Get Numbers Of UnRead Notification
        public async Task<BaseResponseModel> GetNumbersOfUnReadNotification(int accountId)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(accountId, filter: t => t.Status == (int)AccountStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);

            var notifications = await _unitOfWork.NotificationRepo.GetAsync(filter: t => t.AccountId == accountId && t.IsRead == false && t.IsDeleted == false);
            var notiList = notifications.ToList().Count;

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_NOT_READ_NOTIFICATION_SUCCESS,
                Result = notiList
            };
        }
        #endregion

        #region Make All Notifications Is Read
        public async Task<BaseResponseModel> MakeAllNotificationsIsReadAsync(int accountId)
        {
            var notifications = await _unitOfWork.NotificationRepo.GetAsync(filter: t => t.AccountId == accountId && t.IsRead == false && t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.GET_NOTIFICATION_NOT_EXIST);
            foreach (var noti in notifications)
            {
                noti.IsRead = true;
                noti.ReadAt = DateTime.UtcNow;

                _unitOfWork.NotificationRepo.Update(noti); 
            }
            await _unitOfWork.SaveChangesAsync();


            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.IS_READ_SUCCESS
            };
        }

        #endregion

        #region Make Notifications IsRead
        public async Task<BaseResponseModel> MakeNotificationsIsReadAsync(int id)
        {
            var notifications = (await _unitOfWork.NotificationRepo.GetAsync(filter: t => t.Id == id && t.IsRead == false && t.IsDeleted == false)).FirstOrDefault()
                ?? throw new NotExistsException(ConstantResponse.GET_NOTIFICATION_NOT_EXIST);
            notifications.IsRead = true;
            notifications.ReadAt = DateTime.UtcNow;

            _unitOfWork.NotificationRepo.Update(notifications);
            await _unitOfWork.SaveChangesAsync();


            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.IS_READ_SUCCESS
            };
        }
        #endregion

        #region Send Notification To All
        public async Task SendNotificationToAll(NotificationModel notification)
        {
            var users = await _unitOfWork.RoleAssignmentRepo.GetV2Async(
                filter: t => t.Role.Name.ToLower() != RoleEnum.Admin.ToString().ToLower()
                        && t.IsDeleted == false && t.Account.Status == (int)AccountStatus.Active,
                include: query => query.Include(t => t.Account).Include(q => q.Role));

            foreach (var user in users)
            {
                var noti = _mapper.Map<Notification>(notification);
                noti.AccountId = user.Id;
                await _unitOfWork.NotificationRepo.AddAsync(noti);
            }
            await _unitOfWork.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
        #endregion

        #region Send Notification To User
        public async Task SendNotificationToUser(int accountId, NotificationModel notification)
        {
            var noti = _mapper.Map<Notification>(notification);
            noti.AccountId = accountId;
            await _unitOfWork.NotificationRepo.AddAsync(noti);

            await _hubContext.Clients.User(accountId.ToString()).SendAsync("ReceiveNotification", notification);
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion
    }
}
