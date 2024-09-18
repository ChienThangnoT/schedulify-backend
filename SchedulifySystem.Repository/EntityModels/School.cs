using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class School : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PrincipalName { get; set; }

        public ICollection<TeacherGroup> TeacherGroups { get; set; } = new List<TeacherGroup>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<SubjectGroup> SubjectGroups { get; set; } = new List<SubjectGroup>();
        public ICollection<Building> Buildings { get; set; } = new List<Building>();
        public ICollection<Term> Terms { get; set; } = new List<Term>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
        public ICollection<ClassGroup> ClassGroups { get; set; } = new List<ClassGroup>();
        public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<SchoolSchedule> SchoolSchedules { get; set; } = new List<SchoolSchedule>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    }
}
