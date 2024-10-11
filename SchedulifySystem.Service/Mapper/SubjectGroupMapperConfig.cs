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
            CreateMap<SubjectGroupAddModel, SubjectGroup>().ForMember(dest => dest.CreateDate, opt => opt.MapFrom(_ => DateTime.UtcNow)).ReverseMap();
        }
    }
}
