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
            CreateMap<TeacherAssignment, TeacherAssignmentViewModel>();
            
            CreateMap<AddTeacherAssignmentModel, TeacherAssignment>()
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTeacherAssignmentModel, TeacherAssignment>()
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
