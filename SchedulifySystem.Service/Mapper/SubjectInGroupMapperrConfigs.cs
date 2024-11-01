using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
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
            CreateMap<SubjectInGroup, SubjectInGroupViewModel>();
            CreateMap<SubjectInGroup, SubjectInGroupUpdateModel>()
                .ForMember(dest => dest.SubjectInGroupId, otp => otp.MapFrom(s => s.Id)).ReverseMap();
            //CreateMap<SubjectInGroup, SubjectInGroupViewDetailModel>()
            //   .ForMember(dest => dest.SubjectName,
            //    opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
            //   .ReverseMap();
            CreateMap<SubjectInGroup, SubjectScheduleModel>()
                .IncludeMembers(sig => sig.Subject).ReverseMap();

        }
    }
}
