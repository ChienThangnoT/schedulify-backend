﻿using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class CreateStudentClassModel
    {
        public string? Name { get; set; }
        public int? HomeroomTeacherId { get; set; }
        public int? SchoolId { get; set; }
        public int? SchoolYearId { get; set; }
        public int? MainSession { get; set; }
        public int? GradeId { get; set; }
    }
}
