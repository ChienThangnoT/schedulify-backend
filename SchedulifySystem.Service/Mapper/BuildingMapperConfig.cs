using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.BusinessModels.ClassGroupBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Mapper
{
    public partial class MapperConfigs : Profile
    {
        partial void BuildingMapperConfig()
        {
            CreateMap<AddBuildingModel, Building>()
                .ForMember(dest => dest.SchoolId, opt => opt.MapFrom((src, dest, destMember, context) => (int)context.Items["schoolId"]))
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(_ => DateTime.UtcNow)).ReverseMap();

            CreateMap<UpdateBuildingModel, Building>()
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Building, BuildingViewModel>();
        }
    }
}
