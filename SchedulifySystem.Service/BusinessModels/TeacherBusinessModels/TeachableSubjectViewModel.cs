﻿using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeachableSubjectViewModel
    {
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? Abbreviation { get; set; }
        public int AppropriateLevel { get; set; }
        public List<EGrade> Grade { get; set; }
        public bool IsMain { get; set; }
    }
}
