using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class SubjectConfig : BaseEntity
    {
        public int SubjectId { get; set; }
        public int CurriculumId { get; set; }
        public int StudentClassId { get; set; }
        public int SchoolScheduleId { get; set; }
        public bool IsMainSession { get; set; }
        public int Value { get; set; }
        public int ConfigAttributeId { get; set; }

        public Subject? Subject { get; set; }
        public Curriculum? Curriculum { get; set; }
        public StudentClass? StudentClass { get; set; }
        public SchoolSchedule? SchoolSchedule { get; set; }
        public ConfigAttribute? ConfigAttribute { get; set; }
    }
}
