﻿using Microsoft.AspNetCore.SignalR;
using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToAll(Notification notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }

        public async Task SendNotificationToUser(int accountId, Notification notification)
        {
            await Clients.User(accountId.ToString()).SendAsync("ReceiveNotification", notification);
        }
    }
}
