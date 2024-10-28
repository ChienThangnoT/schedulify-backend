using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void SchoolYearMapperConfig()
        {
            CreateMap<SchoolYear, SchoolYearViewModel>();
        }
    }
}
