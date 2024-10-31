using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
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
            CreateMap<FreeTimetablePeriodScheduleModel, ClassPeriodScheduleModel>();

            CreateMap<TimetableIndividual, SchoolSchedule>()
            .ForMember(dest => dest.ClassSchedules, otp => otp.MapFrom(src => src.TimetableUnits.Select(p => new ClassSchedule() 
            { 
                StudentClassId = p.ClassId,
                Name = $"Thời khóa biểu lớp {p.ClassName}",
                CreateDate = DateTime.UtcNow,
                StudentClassName = p.ClassName,
                ClassPeriods = src.TimetableUnits.Where(cp => cp.ClassId == p.ClassId).Select(cp => new ClassPeriod() 
                {
                    StartAt = cp.StartAt,
                    Priority = (int) cp.Priority,

                }).ToList()
            }
            )));

        }
    }
}
