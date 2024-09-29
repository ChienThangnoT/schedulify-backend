using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;
using SchedulifySystem.Service.ViewModels.ResponseModels.TeacherResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void TeacherMapperConfig()
        {
            CreateMap<Teacher, TeacherResponseModel>();
            CreateMap<CreateTeacherRequestModel, Teacher>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateOnly.Parse(src.DateOfBirth)))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateTeacherRequestModel, Teacher>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateOnly.Parse(src.DateOfBirth)))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
