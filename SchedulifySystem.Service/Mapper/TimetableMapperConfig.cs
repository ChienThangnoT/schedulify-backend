using AutoMapper;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.ClassScheduleBusinessModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.SchoolBusinessModels;
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

            CreateMap<SchoolSchedule, SchoolScheduleDetailsViewModel>()
                .ForMember(dest => dest.TermName, otp => otp.MapFrom(src => src.Term.Name))
                .ForMember(dest => dest.StartWeek, otp => otp.MapFrom(src => src.StartWeek))
                .ForMember(dest => dest.EndWeek, otp => otp.MapFrom(src => src.EndWeek))
                .ForMember(dest => dest.ClassSchedules, otp => otp.MapFrom(src => src.ClassSchedules));

            CreateMap<SchoolScheduleDetailsViewModel, SchoolSchedule>()
                .ForMember(dest => dest.StartWeek, otp => otp.MapFrom(src => src.StartWeek))
                .ForMember(dest => dest.EndWeek, otp => otp.MapFrom(src => src.EndWeek))
                .ForMember(dest => dest.SchoolId, otp => otp.MapFrom(src => src.SchoolId))
                .ForMember(dest => dest.TermId, otp => otp.MapFrom(src => src.TermId))
                .ForMember(dest => dest.Name, otp => otp.MapFrom(src => src.Name ?? "Thời khóa biểu không tên"))
                .ForMember(dest => dest.FitnessPoint, otp => otp.MapFrom(src => src.FitnessPoint))
                .ForMember(dest => dest.ClassSchedules, otp => otp.MapFrom(src => src.ClassSchedules.Select(cs => new ClassSchedule
                {
                    StudentClassId = cs.StudentClassId,
                    Name = cs.Name ?? $"Thời khóa biểu lớp {cs.StudentClassName}",
                    CreateDate = DateTime.UtcNow,
                    StudentClassName = cs.StudentClassName,
                    ClassPeriods = cs.ClassPeriods.Select(cp => new ClassPeriod
                    {
                        StartAt = cp.StartAt,
                        Priority = (int)cp.Priority,
                        TeacherAbbreviation = cp.TeacherAbbreviation,
                        TeacherAssignmentId = cp.TeacherAssignmentId,
                        TeacherId = cp.TeacherId,
                        SubjectAbbreviation = cp.SubjectAbbreviation,
                        SubjectId = cp.SubjectId,
                        RoomId = cp.RoomId,
                        RoomCode = cp.RoomCode,
                        DateOfWeek = cp.DateOfWeek,
                        CreateDate = DateTime.UtcNow,
                    }).ToList()
                })));


            CreateMap<ClassSchedule, ClassScheduleViewModel>()
                .ForMember(dest => dest.ClassPeriods, otp => otp.MapFrom(src => src.ClassPeriods));

            CreateMap<ClassPeriod, ClassPeriodViewModel>();
            CreateMap<TimetableIndividual, TimetableIndividual>();

            CreateMap<SchoolSchedule, SchoolScheduleViewModel>()
                .ForMember(dest => dest.TermName, otp => otp.MapFrom(src => src.Term.Name))
                .ForMember(dest => dest.StartYear, otp => otp.MapFrom(src => src.SchoolYear.StartYear))
                .ForMember(dest => dest.EndYear, otp => otp.MapFrom(src => src.SchoolYear.EndYear));

            CreateMap<TimetableIndividual, SchoolScheduleDetailsViewModel>()
            .ForMember(dest => dest.ClassSchedules, opt => opt.MapFrom(src =>
                src.Classes.Select(c => new ClassScheduleViewModel()
                {
                    StudentClassId = c.Id,
                    Name = $"Thời khóa biểu lớp {c.Name}",
                    CreateDate = DateTime.UtcNow,
                    StudentClassName = c.Name,
                    ClassPeriods = src.TimetableUnits
                        .Where(cp => cp.ClassId == c.Id)
                        .Select(cp => new ClassPeriodViewModel()
                        {
                            StartAt = cp.StartAt,
                            Priority = cp.Priority,
                            TeacherAbbreviation = cp.TeacherAbbreviation,
                            TeacherAssignmentId = cp.TeacherAssignmentId,
                            TeacherId = cp.TeacherId,
                            SubjectAbbreviation = cp.SubjectAbbreviation,
                            SubjectId = cp.SubjectId,
                            CreateDate = DateTime.UtcNow,
                            DateOfWeek = cp.StartAt / 10
                        })
                        .OrderBy(cp => cp.StartAt)
                        .ToList()
                })
            ));
        }

    }
}
