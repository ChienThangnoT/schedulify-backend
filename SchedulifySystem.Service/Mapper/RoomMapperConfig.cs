using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void RoomMapperConfig()
        {
            CreateMap<AddRoomModel, Room>()
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();

            CreateMap<UpdateRoomModel, Room>()
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Room, RoomViewModel > ();
        }

    }
}
