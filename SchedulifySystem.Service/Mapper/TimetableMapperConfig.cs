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
            .ForMember(dest => dest.ClassSchedules, otp => otp.MapFrom(src => src.Classes.Select(c => new ClassSchedule() 
            { 
                StudentClassId = c.Id,
                Name = $"Thời khóa biểu lớp {c.Name}",
                CreateDate = DateTime.UtcNow,
                StudentClassName = c.Name,
                ClassPeriods = src.TimetableUnits.Where(cp => cp.ClassId == c.Id).Select(cp => new ClassPeriod() 
                {
                    StartAt = cp.StartAt,
                    Priority = (int) cp.Priority,
                    TeacherAbbreviation = cp.TeacherAbbreviation,
                    TeacherAssignmentId = cp.TeacherAssignmentId,
                    TeacherId = cp.TeacherId,
                    SubjectAbbreviation = cp.SubjectAbbreviation,
                    SubjectId = cp.SubjectId,
                    CreateDate = DateTime.UtcNow,
                    DateOfWeek = cp.StartAt / 10,
                }).OrderBy(cp => cp.StartAt).ToList()
            }
            )));

        }
    }
}
