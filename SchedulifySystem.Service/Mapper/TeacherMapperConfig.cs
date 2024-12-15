using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeachableSubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
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
        partial void TeacherMapperConfig()
        {
            CreateMap<Teacher, TeacherViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.Gender, otp => otp.MapFrom(src => src.Gender == (int)Gender.Female ? "Female" : "Male"))
                .ForMember(dest => dest.TeachableSubjects, opt => opt.MapFrom(src => src.TeachableSubjects))
                .ForMember(dest => dest.IsHomeRoomTeacher, opt => opt.MapFrom(src => src.StudentClasses.Any()))
                .ForMember(dest => dest.HomeRoomTeacherOfClass, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Name))
                .ForMember(dest => dest.StudentClassId, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Id));

            CreateMap<Teacher, TeacherAccountViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForMember(dest => dest.Gender, otp => otp.MapFrom(src => src.Gender == (int)Gender.Female ? "Female" : "Male")).ReverseMap();

            CreateMap<TeachableSubject, TeachableSubjectViewModel>()
                .ForMember(dest => dest.SubjectId, otp => otp.MapFrom(src => src.Subject.Id) )
                .ForMember(dest => dest.SubjectName, otp => otp.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.Abbreviation, otp => otp.MapFrom(src => src.Subject.Abbreviation))
                .ForMember(dest => dest.ListApproriateLevelByGrades, opt => opt.MapFrom(src => new List<ListApproriateLevelByGrade>
                {
                    new ListApproriateLevelByGrade
                    {
                        AppropriateLevel = (EAppropriateLevel)src.AppropriateLevel,
                        Grade = (EGrade)src.Grade
                    }
                }));

            CreateMap<TeachableSubject, TeachableSubjectTimetableViewModel>()
                .ForMember(dest => dest.SubjectId, otp => otp.MapFrom(src => src.Subject.Id))
                .ForMember(dest => dest.SubjectName, otp => otp.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.Abbreviation, otp => otp.MapFrom(src => src.Subject.Abbreviation))
                .ForMember(dest => dest.TeacherId, otp => otp.MapFrom(src => src.Teacher.Id))
                .ForMember(dest => dest.FirstName, otp => otp.MapFrom(src => src.Teacher.FirstName))
                .ForMember(dest => dest.LastName, otp => otp.MapFrom(src => src.Teacher.LastName))
                .ForMember(dest => dest.ListApproriateLevelByGrades, opt => opt.MapFrom(src => new List<ListApproriateLevelByGrade>
                {
                    new ListApproriateLevelByGrade
                    {

                        Id = src.Id,
                        IsMain = src.IsMain,
                        AppropriateLevel = (EAppropriateLevel)src.AppropriateLevel,
                        Grade = (EGrade)src.Grade
                    }
                }));

            CreateMap<CreateTeacherModel, Teacher>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CreateListTeacherModel, Teacher>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTeacherModel, Teacher>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<TeachableSubject, TeachableSubjectDetailsViewModel>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.SubjectAbreviation, opt => opt.MapFrom(src => src.Subject.Abbreviation))
                .ForMember(dest => dest.TeacherAbreviation, opt => opt.MapFrom(src => src.Teacher.Abbreviation))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"));
        }
    }
}
