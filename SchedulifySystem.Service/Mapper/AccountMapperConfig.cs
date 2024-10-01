using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void AccountMapperConfig()
        {
            CreateMap<CreateSchoolManagerModel, Account>().ReverseMap();
            CreateMap<Account, AccountViewModel>()
            .ForMember(dest => dest.SchoolName,
                opt => opt.MapFrom(src => src.School != null ? src.School.Name : string.Empty))
            .ReverseMap();
        }
    }
}
