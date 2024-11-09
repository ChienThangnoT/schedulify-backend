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
        public async Task<BaseResponseModel> GetAllNotificationsByAccountIdAsync(int accountId, bool? isRead)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(accountId, filter: t => t.Status == (int)AccountStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.ACCOUNT_NOT_EXIST);

            var notification = await _unitOfWork.NotificationRepo.GetPaginationAsync(
                filter: t => t.AccountId == accountId && (isRead == null || t.IsRead == isRead) && t.IsDeleted == false,
                orderBy: query => query.OrderByDescending(t => t.CreateDate));

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

        public Task<BaseResponseModel> MakeAllNotificationsIsReadAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> MakeNotificationsIsReadAsync(int id)
        {
            throw new NotImplementedException();
        }

        #region Send Notification To All
        public async Task SendNotificationToAll(NotificationModel notification)
        {
            var users = await _unitOfWork.RoleAssignmentRepo.GetV2Async(
                filter: t => t.Role.Name.ToLower() != RoleEnum.Admin.ToString().ToLower() 
                        && t.IsDeleted == false && t.Account.Status == (int)AccountStatus.Active,
                include: query => query.Include(t => t.Account).Include(q => q.Role));

            foreach(var user in users)
            {
                var noti = _mapper.Map<Notification>(notification);
                noti.AccountId = user.Id;
                await _unitOfWork.NotificationRepo.AddAsync(noti);
            }
            await _unitOfWork.SaveChangesAsync();
            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
        #endregion

        #region
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
