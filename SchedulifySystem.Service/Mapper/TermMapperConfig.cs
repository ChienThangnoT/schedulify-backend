using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void TermMapperConfig()
        {
            CreateMap<Term, TermViewModel>()
                .ForMember(dest => dest.SchoolYearCode,
                opt => opt.MapFrom(src => src.SchoolYear != null ? src.SchoolYear.SchoolYearCode : string.Empty))
                .ForMember(dest => dest.SchoolYearStart,
                opt => opt.MapFrom(src => src.SchoolYear != null ? src.SchoolYear.StartYear : string.Empty))
                .ForMember(dest => dest.SchoolYearEnd,
                opt => opt.MapFrom(src => src.SchoolYear != null ? src.SchoolYear.EndYear : string.Empty))
                .ReverseMap();
        }
    }
}
