using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.NotificationBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void NotificationMapperConfig()
        {
            CreateMap<NotificationModel, Notification>().ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow)).ReverseMap();
        }
    }
}
