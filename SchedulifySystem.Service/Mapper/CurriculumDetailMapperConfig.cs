using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
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
        partial void CurriculumDetailMapperConfig()
        {
            CreateMap<CurriculumDetail, CurriculumDetailViewModel>();
            CreateMap<CurriculumDetail, CurriculumDetailUpdateModel>()
                .ForMember(dest => dest.CurriculumDetailId, otp => otp.MapFrom(s => s.Id)).ReverseMap();
            CreateMap<CurriculumDetail, SubjectViewDetailModel>()
               .ForMember(dest => dest.SubjectName,
                opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
               .ForMember(dest => dest.SubjectId,
                opt => opt.MapFrom(src => src.Subject.Id))
               .ForMember(dest => dest.Abbreviation,
                opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Abbreviation : string.Empty))
               .ForMember(dest => dest.IsRequired,
               opt => opt.MapFrom(src => src.Subject.IsRequired ))
               .ForMember(dest => dest.Description,
               opt => opt.MapFrom(src => src.Subject.Description))
               .ForMember(dest => dest.SubjectGroupType,
                opt => opt.MapFrom(src => src.Subject.SubjectGroupType ))
               .ForMember(dest => dest.TotalSlotInYear,
                opt => opt.MapFrom(src => src.Subject.TotalSlotInYear))
               .ReverseMap();
            //CreateMap<CurriculumDetail, SubjectScheduleModel>()
            //    .IncludeMembers(sig => sig.Subject).ReverseMap();

        }
    }
}
