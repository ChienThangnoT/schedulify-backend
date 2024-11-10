﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulifySystem.Service.BusinessModels.NotificationBusinessModels;
using SchedulifySystem.Service.Services.Implements;
using SchedulifySystem.Service.Services.Interfaces;

namespace SchedulifySystem.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{accountId}")]
        public Task<IActionResult> GetAllNotificationsByAccountIdAsync(int accountId, bool? isRead)
        {
            return ValidateAndExecute(() => _notificationService.GetAllNotificationsByAccountIdAsync(accountId, isRead));
        }
        
        [HttpGet("{accountId}/number-unread")]
        public Task<IActionResult> GetNumbersOfUnReadNotification(int accountId)
        {
            return ValidateAndExecute(() => _notificationService.GetNumbersOfUnReadNotification(accountId));
        }

        [HttpPost("send-to-all")]
        public async Task<IActionResult> SendToAll([FromBody] NotificationModel notification)
        {
            await _notificationService.SendNotificationToAll(notification);
            return Ok(new { status = "Notification sent to all users"});
        }

        [HttpPost("send-to-user/{accountId}")]
        public async Task<IActionResult> SendToUser(int accountId, [FromBody] NotificationModel notification)
        {
            await _notificationService.SendNotificationToUser(accountId, notification);
            return Ok(new { status = $"Notification sent to user {accountId}" });
        }
    }
}