using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubmitRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void SubmitRequestMapperConfig()
        {
            CreateMap<SubmitSendRequestModel, SubmitRequest>()
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            
            CreateMap<SubmitRequest, SubmitRequestViewModel>()
                .ForMember(dest => dest.SchoolYearCode,
                 opt => opt.MapFrom(src => src.SchoolYear != null ? src.SchoolYear.SchoolYearCode : string.Empty))
                .ForMember(dest => dest.TeacherFirstName,
                 opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FirstName : string.Empty))
                 .ForMember(dest => dest.TeacherLastName,
                 opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.LastName : string.Empty))
                .ReverseMap();
        }
    }
}
