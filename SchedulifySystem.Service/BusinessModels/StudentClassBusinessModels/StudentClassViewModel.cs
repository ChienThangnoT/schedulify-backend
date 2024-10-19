﻿using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels
{
    public class StudentClassViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public int HomeroomTeacherId { get; set; }
        public string? HomeroomTeacherName { get; set; }
        public string? HomeroomTeacherAbbreviation { get; set; }
        public int MainSession { get; set; }
        public string? MainSessionText { get; set; }
        public int Grade { get; set; }
        public bool IsFullDay { get; set; }
        public int PeriodCount { get; set; }
        public int? SubjectGroupId { get; set; }
    }
}
