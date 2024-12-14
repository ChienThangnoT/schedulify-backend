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
              .ForMember(dest => dest.StudentClassGroupName,
                         opt => opt.MapFrom(src => src.StudentClassGroup != null ? src.StudentClassGroup.GroupName : string.Empty))
              .ForMember(dest => dest.StudentClassGroupCode,
                         opt => opt.MapFrom(src => src.StudentClassGroup != null ? src.StudentClassGroup.StudentClassGroupCode : string.Empty))
              .ForMember(dest => dest.StudentClassGroupId,
                         opt => opt.MapFrom(src => src.StudentClassGroup != null ? (int?)src.StudentClassGroup.Id : null))
              .ForMember(dest => dest.CurriculumName,
                         opt => opt.MapFrom(src => src.StudentClassGroup.Curriculum != null ? src.StudentClassGroup.Curriculum.CurriculumName : string.Empty))
               .ForMember(dest => dest.CurriculumCode,
                         opt => opt.MapFrom(src => src.StudentClassGroup.Curriculum != null ? src.StudentClassGroup.Curriculum.CurriculumCode : string.Empty))
               .ForMember(dest => dest.CurriculumId,
                         opt => opt.MapFrom(src => src.StudentClassGroup.Curriculum != null ? src.StudentClassGroup.Curriculum.Id : 0))
               .ForMember(dest => dest.PeriodCount,
                         opt => opt.MapFrom(src => src.PeriodCount != 0 ? src.PeriodCount/2 : 0))
              .ForMember(dest => dest.RoomName,
                         opt => opt.MapFrom(src => src.Room != null ? src.Room.Name : string.Empty))
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

            CreateMap<StudentClass, StudentClassViewName>();
        }
    }
}
