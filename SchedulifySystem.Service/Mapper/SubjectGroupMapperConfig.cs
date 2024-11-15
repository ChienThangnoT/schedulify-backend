using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels;
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
        partial void SubjectGroupMapperConfig()
        {
            CreateMap<CurriculumAddModel, Curriculum>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<CurriculumUpdateModel, Curriculum>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<Curriculum, CurriculumDetailViewModel>()
               .ForMember(dest => dest.SchoolName,
                opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
               .ReverseMap();

            CreateMap<Curriculum, CurriculumViewDetailModel>()
                .ForMember(dest => dest.SchoolYear,
                opt => opt.MapFrom(src => src.SchoolYear != null ? $"{src.SchoolYear.StartYear} - {src.SchoolYear.EndYear}":""))
                .ReverseMap();
        }
    }
}
