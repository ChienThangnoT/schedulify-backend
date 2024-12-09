using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes
{
    public class TimetableRootIndividual(ETimetableFlag[,] timetableFlag,
        List<ClassPeriodScheduleModel> timetableUnits,
        List<ClassScheduleModel> classes,
        List<TeacherScheduleModel> teachers,
        Dictionary<int, List<SubjectScheduleModel>> doubleSubjectsByGroup) : TimetableIndividual(timetableFlag, timetableUnits, classes, teachers, doubleSubjectsByGroup)
    {

    }
}
