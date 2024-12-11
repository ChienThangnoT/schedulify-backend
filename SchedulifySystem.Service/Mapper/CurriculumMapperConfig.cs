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
        partial void CurriculumMapperConfig()
        {
            CreateMap<CurriculumAddModel, Curriculum>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<CurriculumUpdateModel, Curriculum>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<Curriculum, CurriculumViewModel>();

            CreateMap<Curriculum, CurriculumViewDetailModel>()
               .ReverseMap();
        }
    }
}
