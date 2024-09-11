using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public class School
    {
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PrincipalName { get; set; }

        public ICollection<TeacherGroup> teacherGroups { get; set; }
        public ICollection<Teacher> teachers { get; set; }
        public ICollection<Building> Buildings { get; set; }
        public ICollection<Term> Terms { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<Curriculum> Curriculums { get; set; }
        public ICollection<ClassGroup> ClassGroups { get; set; }
        public ICollection<RoomType> RoomTypes { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; }
        public ICollection<SchoolSchedule> SchoolSchedules { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
