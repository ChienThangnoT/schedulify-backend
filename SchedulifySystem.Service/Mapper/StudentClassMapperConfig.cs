using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;


namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void StudentClassMapperConfig()
        {
            CreateMap<StudentClassInGroup, StudentClassViewModel>()
                .ForMember(dest => dest.HomeroomTeacherName,
                otp => otp.MapFrom(src => src.StudentClass.Teacher == null ? "" : $"{src.StudentClass.Teacher.FirstName} {src.StudentClass.Teacher.LastName}"))
                .ForMember(dest => dest.Name, otp => otp.MapFrom(src => src.StudentClass == null ? "" : src.StudentClass.Name))
                .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src.StudentClass.Id))
                .ForMember(dest => dest.MainSession, otp => otp.MapFrom(src => src.StudentClass.MainSession))
                .ForMember(dest => dest.HomeroomTeacherId, otp => otp.MapFrom(src => src.StudentClass.HomeroomTeacherId))
                .ForMember(dest => dest.GradeName, otp => otp.MapFrom(src => src.ClassGroup.Name))
                .ForMember(dest => dest.GradeId, otp => otp.MapFrom(src => src.ClassGroup.Id))
                .ForMember(dest => dest.IsDeleted, otp => otp.MapFrom(src => src.StudentClass.IsDeleted))
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.StudentClass.CreateDate))
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.StudentClass.UpdateDate))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.StudentClass.Status));

            CreateMap<CreateStudentClassModel, StudentClass>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToUpper()));

        }
    }
}
