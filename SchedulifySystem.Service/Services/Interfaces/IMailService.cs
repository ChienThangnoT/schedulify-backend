using AutoMapper.Internal;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequest mailRequest);

    }
}
