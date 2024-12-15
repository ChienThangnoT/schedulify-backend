using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeacherAccountViewModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Abbreviation { get; set; }
        public string? Email { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public TeacherRole TeacherRole { get; set; }
        public bool IsTeacherHeadDepartment { get; set; }
        public TeacherStatus TeacherStatus { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHaveAccount { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public int? AccountId { get; set; }
    }
}
