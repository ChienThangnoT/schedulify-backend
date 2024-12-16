using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.DistrictBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void DistrictMapperConfig()
        {
            CreateMap<DistrictViewModel, District>().ReverseMap();
            CreateMap<DistrictAddModel, District>();
            CreateMap<DistrictUpdateModel, District>();
        }
    }
}
