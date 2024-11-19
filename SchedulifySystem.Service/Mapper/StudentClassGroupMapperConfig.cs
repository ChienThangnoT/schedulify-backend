using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{

    public partial class MapperConfigs : Profile
    {
        partial void StudentClassGroupMapperConfig()
        {
            CreateMap<AddStudentClassGroupModel, StudentClassGroup>()
                .ForMember(dest => dest.StudentClassGroupCode, otp => otp.MapFrom(src => src.StudentClassGroupCode.ToUpper()))
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<UpdateStudentClassGroupModel, StudentClassGroup>()
                .ForMember(dest => dest.StudentClassGroupCode, otp => otp.MapFrom(src => src.StudentClassGroupCode.ToUpper()))
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<StudentClassGroup, StudentClassGroupViewModel>()
                .ForMember(dest => dest.Classes, otp => otp.MapFrom(src => src.StudentClasses))
                .ReverseMap();
            CreateMap<UpdateTeacherModel, Teacher>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
