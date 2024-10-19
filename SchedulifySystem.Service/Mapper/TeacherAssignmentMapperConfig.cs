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
                    .MapFrom(t => t.Teacher != null ? t.Teacher.FirstName: string.Empty ))
                .ForMember(dest => dest.TeacherLastName, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.LastName : string.Empty))
                .ForMember(dest => dest.TeacherAbbreviation, otp => otp
                    .MapFrom(t => t.Teacher != null ? t.Teacher.Abbreviation: string.Empty))
                .ForMember(dest => dest.SubjectName, otp => otp
                    .MapFrom(t => t.Subject != null ? t.Subject.SubjectName : string.Empty)).ReverseMap();

            CreateMap<AddTeacherAssignmentModel, TeacherAssignment>()
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();

            CreateMap<UpdateTeacherAssignmentModel, TeacherAssignment>()
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();
        }
    }
}
