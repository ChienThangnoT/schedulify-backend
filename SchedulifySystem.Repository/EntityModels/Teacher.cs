using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int DepartmentId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int SchoolId { get; set; }
        public int TeacherGroupId { get; set; }
        public int Status { get; set; }

        public Department Department { get; set; }
        public School School { get; set; }
        public TeacherGroup Group { get; set; }
        public ICollection<TeachableSubject> TeachableSubjects { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; }
        public ICollection<ClassPeriod> ClassPeriods { get; set; }
        public ICollection<TeacherConfig> TeacherConfigs { get; set; }
        public ICollection<TeacherUnavailability> TeacherUnavailabilities { get; set; }
    }

}
