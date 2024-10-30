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
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RoomCode, otp => otp.MapFrom(src => src.RoomCode.ToUpper()))
                .ReverseMap();

            CreateMap<UpdateRoomModel, Room>()
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RoomCode, otp => otp.MapFrom(src => src.RoomCode.ToUpper()));

            CreateMap<Room, RoomViewModel > ()
               .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.RoomSubjects.Select(rs => new RoomSubjectViewModel
               {
                   SubjectId = (int)rs.SubjectId, 
                   SubjectName = rs.Subject.SubjectName,
                   Abbreviation = rs.Subject.Abbreviation

               })));

        }

    }
}
