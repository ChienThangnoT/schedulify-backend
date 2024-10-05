using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void SubjectMapperConfig()
        {
            CreateMap<SubjectAddModel, Subject>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<Subject, SubjectViewModel>()
                .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
                .ReverseMap();
        }
    }
}
