using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeacherScheduleModel : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Abbreviation { get; set; }
        public string? Email { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int SchoolId { get; set; }
        public TeacherRole TeacherRole { get; set; }
        public TeacherStatus Status { get; set; }
        public string? Phone { get; set; }

        public TeacherScheduleModel(Teacher teacher)
        {
            FirstName = teacher.FirstName;
            LastName = teacher.LastName;
            Abbreviation = teacher.Abbreviation;
            Email = teacher.Email;
            Gender = (Gender)teacher.Gender;
            DepartmentId = teacher.DepartmentId;
            DateOfBirth = teacher.DateOfBirth;
            SchoolId = teacher.SchoolId;
            TeacherRole = (TeacherRole)teacher.TeacherRole;
            Status = (TeacherStatus)teacher.Status;
            Phone = teacher.Phone;
            Id = teacher.Id;
        }
    }
}
