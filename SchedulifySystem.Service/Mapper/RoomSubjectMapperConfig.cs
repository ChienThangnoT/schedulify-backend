using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void RoomSubjectMapperConfig()
        {
            CreateMap<RoomSubject, RoomSubjectScheduleModel>()
                .ReverseMap();

            CreateMap<RoomSubjectAddModel, RoomSubject>()
                 .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                 .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade))
                .ReverseMap();

            CreateMap<RoomSubjectUpdateModel, RoomSubject>()
                 .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                 .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade))
                .ReverseMap();
        }
    }
}
