using AutoMapper;
using SchedulifySystem.Repository.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        public MapperConfigs()
        {
            //add map here
            TeacherMapperConfig();
            //create map between pagination
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
        }
        partial void TeacherMapperConfig();
    }
}
