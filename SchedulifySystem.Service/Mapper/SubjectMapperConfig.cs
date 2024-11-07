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
            CreateMap<SubjectUpdateModel, Subject>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<SubjectAddListModel, Subject>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<Subject, SubjectViewModel>()
                .ForMember(dest => dest.SchoolYearCode,
                 opt => opt.MapFrom(src => src.SchoolYear != null ? src.SchoolYear.SchoolYearCode : string.Empty))
                .ReverseMap();
            CreateMap<Subject, SubjectScheduleModel>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Id)).ReverseMap();
        }
    }
}
