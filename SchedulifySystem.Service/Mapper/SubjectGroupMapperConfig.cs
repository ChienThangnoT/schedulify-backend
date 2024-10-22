using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
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
            CreateMap<SubjectGroupAddModel, SubjectGroup>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.GroupCode, opt =>  opt.MapFrom(src => src.GroupCode.ToUpper()))
                .ReverseMap();

            CreateMap<SubjectGroupUpdateModel, SubjectGroup>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode.ToUpper()))
                .ReverseMap();

            CreateMap<SubjectGroup, SubjectGroupViewModel>()
               .ForMember(dest => dest.SchoolName,
                opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
               .ReverseMap();

            CreateMap<SubjectGroup, SubjectGroupViewDetailModel>()
                .ForMember(dest => dest.SchoolName,
                opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
                .ForMember(dest => dest.SchoolYear,
                opt => opt.MapFrom(src => src.SchoolYear != null ? $"{src.SchoolYear.StartYear} - {src.SchoolYear.EndYear}":""))
                .ReverseMap();
        }
    }
}
