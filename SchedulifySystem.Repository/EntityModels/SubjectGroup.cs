﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SubjectGroup : BaseEntity
    {
        public string? GroupName { get; set; }
        public int SchoolId { get; set; }
        public string? GroupDescription { get; set; }
        public int SubjectGroupTypeId { get; set; }
        public string? GroupCode { get; set; }
        public School? School { get; set; }
        public SubjectGroupType? SubjectGroupType { get; set; }
        public ICollection<SubjectInGroup> SubjectInGroups { get; set; } = new List<SubjectInGroup>();
        public ICollection<ClassGroup> ClassGroups { get; set; } = new List<ClassGroup>();
        public ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
    }
}
