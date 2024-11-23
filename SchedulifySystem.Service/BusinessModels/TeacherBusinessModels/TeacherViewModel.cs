using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Abbreviation { get; set; }
        public string? Email { get; set; }
        public Gender Gender { get; set; }
        public bool? IsHomeRoomTeacher { get; set; }
        public int? StudentClassId { get; set; }
        public string? HomeRoomTeacherOfClass { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public TeacherRole TeacherRole { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public string? Phone { get; set; }
        public int PeriodCount { get; set; }
        public List<TeachableSubjectViewModel> TeachableSubjects { get; set; }
        
    }
}
