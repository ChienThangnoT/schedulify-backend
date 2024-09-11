using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }

        public ICollection<SubjectInGroup> SubjectInGroups { get; set; }
        public ICollection<SchoolSchedule> SchoolSchedules { get; set; }
        public ICollection<TeachableSubject> TeachableSubjects { get; set;}
        public ICollection<SubjectConfig> SubjectConfigs { get; set;}
    }

}
