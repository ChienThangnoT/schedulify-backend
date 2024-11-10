﻿using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class SubjectGradeModel
    {
        public string SubjectAbreviation {  get; set; }
        public EGrade Grade { get; set; }
    }
}
