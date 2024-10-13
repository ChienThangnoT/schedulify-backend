using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SchoolBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void SchoolMapperConfig()
        {
            CreateMap<School,SchoolViewModel>()
                .ForMember(dest => dest.ProvinceName,
                    opt => opt.MapFrom(src => src.Province != null ? src.Province.Name : string.Empty))
                .ReverseMap();
        }
    }
}
