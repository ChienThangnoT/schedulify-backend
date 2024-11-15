using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void SubjectInGroupMapperrConfigs()
        {
            CreateMap<CurriculumDetail, CurriculumDetailViewModel>();
            CreateMap<CurriculumDetail, CurriculumDetailUpdateModel>()
                .ForMember(dest => dest.CurriculumDetailId, otp => otp.MapFrom(s => s.Id)).ReverseMap();
            //CreateMap<SubjectInGroup, SubjectInGroupViewDetailModel>()
            //   .ForMember(dest => dest.SubjectName,
            //    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
            //   .ReverseMap();
            //CreateMap<CurriculumDetail, SubjectScheduleModel>()
            //    .IncludeMembers(sig => sig.Subject).ReverseMap();

        }
    }
}
