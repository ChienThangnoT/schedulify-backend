using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;


namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void StudentClassMapperConfig()
        {
            CreateMap<StudentClass, StudentClassViewModel>()
              .ForMember(dest => dest.HomeroomTeacherAbbreviation,
                         opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Abbreviation : null))
              .ForMember(dest => dest.HomeroomTeacherName,
                         opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" : null))
              .ForMember(dest => dest.SubjectGroupName,
                         opt => opt.MapFrom(src => src.StudentClassGroup.GroupName))
              .ForMember(dest => dest.MainSessionText,
                         opt => opt.MapFrom(src => src.MainSession == (int)MainSession.Morning ? "Sáng" : "Chiều"));



            CreateMap<CreateStudentClassModel, StudentClass>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForPath(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper())).ReverseMap();

            CreateMap<CreateListStudentClassModel, StudentClass>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForPath(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ReverseMap();

            CreateMap<UpdateStudentClassModel, StudentClass>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForPath(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper()));
        }
    }
}
