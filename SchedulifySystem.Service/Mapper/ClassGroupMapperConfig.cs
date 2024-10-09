using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassGroupBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoleAssignmentBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void ClassGroupMapperConfig()
        {
            CreateMap<ClassGroup, GradeViewModel>();
        }
    }
}
