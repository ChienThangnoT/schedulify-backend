﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class StudentClassInGroup : BaseEntity
    {
        public int StudentClassId { get; set; }
        public int ClassGroupId { get; set; }

        public StudentClass? StudentClass { get; set; }
        public ClassGroup? ClassGroup { get; set; }  
    }
}
