using AutoMapper;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void TimetableMapperConfig()
        {
            CreateMap<FixedPeriodScheduleModel, ClassPeriodScheduleModel>();
            CreateMap<NoAssignPeriodScheduleModel, ClassPeriodScheduleModel>();
        }
    }
}
