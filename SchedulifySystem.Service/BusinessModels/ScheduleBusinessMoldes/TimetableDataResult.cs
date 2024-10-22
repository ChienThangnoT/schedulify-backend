using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class TimetableDataResult
    {
        public List<ClassScheduleModel> Classes { get; set; }
        public List<TeacherScheduleModel> Teachers { get; set; }
        public List<SubjectScheduleModel> Subjects { get; set; }
        public List<TeacherAssigmentScheduleModel> Assignments { get; set; }
        public ETimetableFlag[,] TimetableFlags { get; set; }
    }

}
