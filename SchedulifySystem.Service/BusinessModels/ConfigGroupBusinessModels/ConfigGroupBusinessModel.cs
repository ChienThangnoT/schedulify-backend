using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ConfigGroupBusinessModels
{
    public class ConfigGroupBusinessModel
    {
        public string? Name { get; set; }
        public GroupType GroupType { get; set; }
        public int Status { get; set; }

       
        public bool IsTeacherGroup()
        {
            return GroupType == GroupType.Teacher;  
        }

        public bool IsSubjectGroup()
        {
            return GroupType == GroupType.Subject;
        }
        public bool IsPeriodGroup()
        {
            return GroupType == GroupType.Period;
        }
        public bool IsClassGroup()
        {
            return GroupType == GroupType.Class;
        }

        public bool IsClassroomGroup()
        {
            return GroupType == GroupType.Classroom;
        }

        public bool IsBuildingGroup()
        {
            return GroupType == GroupType.Building;
        }


    }
}
