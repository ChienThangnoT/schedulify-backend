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
            //create map between pagination
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));

            //add teacher mapper config
            TeacherMapperConfig();
            
            //add account mapper config
            AccountMapperConfig();
            
            //add role assignment  mapper config
            RoleAssignmentMapperConfig();

            //add student class mapper config
            StudentClassMapperConfig();

            //add subject mapper config
            SubjectMapperConfig();

            //add building mapper config
            BuildingMapperConfig();

            //add subject group mapper config
            SubjectGroupMapperConfig();

            //add school mapper config
            SchoolMapperConfig();

            //add room mapper config
            RoomMapperConfig();

            //add teacherAssignment mapper config
            TeacherAssignmentMapperConfig();

            //add teacherAssignment mapper config
            SubjectInGroupMapperrConfigs();

            //add Department Mapper Config
            DepartmentMapperConfig();

            //add Timetable Mapper Config
            TimetableMapperConfig();

            //add term Mapper Config
            TermMapperConfig();

            //add School Year Mapper Config
            SchoolYearMapperConfig();

        }
        partial void TeacherMapperConfig();
        partial void AccountMapperConfig();
        partial void RoleAssignmentMapperConfig();
        partial void StudentClassMapperConfig();
        partial void SubjectMapperConfig();
        partial void BuildingMapperConfig();
        partial void RoomMapperConfig();
        partial void SubjectGroupMapperConfig();
        partial void SchoolMapperConfig();
        partial void TeacherAssignmentMapperConfig();
        partial void SubjectInGroupMapperrConfigs();
        partial void DepartmentMapperConfig();
        partial void TimetableMapperConfig();
        partial void TermMapperConfig();
        partial void SchoolYearMapperConfig();


    }
}
