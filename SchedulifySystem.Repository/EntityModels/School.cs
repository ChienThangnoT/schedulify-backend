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
        public int DistrictCode { get; set; }
        public int ProvinceId { get; set; }
        public int? Status { get; set; }
        public Province? Province { get; set; }

        public ICollection<Curriculum> Curriculums { get; set; } = new List<Curriculum>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<StudentClassGroup> StudentClassGroups { get; set; } = new List<StudentClassGroup>();
        public ICollection<Building> Buildings { get; set; } = new List<Building>();
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<SchoolSchedule> SchoolSchedules { get; set; } = new List<SchoolSchedule>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<RoomSubject> RoomSubjects { get; set; } = new List<RoomSubject>();
    }
}
