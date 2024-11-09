using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void TeacherAssignmentMapperConfig()
        {
            CreateMap<TeacherAssignment, TeacherAssignmentViewModel>()
                .ForMember(dest => dest.TeacherFirstName, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.FirstName:null ))
                .ForMember(dest => dest.TeacherLastName, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.LastName : null))
                .ForMember(dest => dest.TeacherAbbreviation, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.Abbreviation: null))
                .ForMember(dest => dest.SubjectName, otp => otp
                    .MapFrom(t => t.Subject != null ? t.Subject.SubjectName : string.Empty)).ReverseMap();

            CreateMap<TeacherAssignment, TeacherAssignmentTermViewModel>()
                .ForMember(dest => dest.TeacherFirstName, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.FirstName : null))
                .ForMember(dest => dest.TeacherLastName, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.LastName : null))
                .ForMember(dest => dest.TeacherAbbreviation, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.Abbreviation : null))
                .ForMember(dest => dest.SubjectName, otp => otp
                    .MapFrom(t => t.Subject != null ? t.Subject.SubjectName : string.Empty)).ReverseMap();

            CreateMap<AssignTeacherAssignmentModel, TeacherAssignment>()
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();

            CreateMap<AssignTeacherAssignmentModel, TeacherAssignment>()
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();
            CreateMap<TeacherAssignment, TeacherAssignment>();
        }
    }
}
