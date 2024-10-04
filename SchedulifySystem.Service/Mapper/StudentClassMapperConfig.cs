using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;


namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void StudentClassMapperConfig()
        {
            CreateMap<StudentClass, StudentClassViewModel>()
                .ForMember(dest => dest.HomeroomTeacherName, 
                otp => otp.MapFrom(src => src.Teacher == null ? "" : $"{src.Teacher.FirstName} {src.Teacher.LastName}"));

            CreateMap<CreateStudentClassModel, StudentClass>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper()));

            CreateMap<UpdateStudentClassBusinessModel, StudentClass>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper()));
        }
    }
}
