using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
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
            CreateMap<SubjectGroupAddModel, StudentClassGroup>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.StudentClassGroupCode, opt =>  opt.MapFrom(src => src.StudentClassGroupCode.ToUpper()))
                .ReverseMap();

            CreateMap<SubjectGroupUpdateModel, StudentClassGroup>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.StudentClassGroupCode, opt => opt.MapFrom(src => src.StudentClassGroupCode.ToUpper()))
                .ReverseMap();

            CreateMap<StudentClassGroup, SubjectGroupViewModel>()
               .ForMember(dest => dest.SchoolName,
                opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
               .ReverseMap();

            CreateMap<StudentClassGroup, SubjectGroupViewDetailModel>()
                .ForMember(dest => dest.SchoolYear,
                opt => opt.MapFrom(src => src.SchoolYear != null ? $"{src.SchoolYear.StartYear} - {src.SchoolYear.EndYear}":""))
                .ReverseMap();
        }
    }
}
