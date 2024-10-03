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
        public string? Gender { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? TeacherGroupName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int TeacherGroupId { get; set; }
        public int TeacherRole { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public string? Phone {  get; set; }
        public List<string> TeachableSubjects { get; set; } = new List<string>();
        
    }
}
