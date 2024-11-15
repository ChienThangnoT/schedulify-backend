using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{

    public partial class MapperConfigs : Profile
    {
        partial void StudentClassGroupMapperConfig()
        {
            CreateMap<StudentClassGroup, AddStudentClassGroupModel>();
        }
    }
}
