using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class CreateListTeacherModel
    {
        [Required(ErrorMessage = "First Name is required."), MaxLength(100, ErrorMessage = "First Name can't be longer than 100 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required."), MaxLength(100, ErrorMessage = "Last Name can't be longer than 100 characters.")]
        public string? LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Abbreviation can't be longer than 50 characters.")]
        public string? Abbreviation { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender value.")]
        public Gender Gender { get; set; }

        public string? DepartmentCode { get; set; }
        
        public DateOnly DateOfBirth { get; set; }


        [EnumDataType(typeof(TeacherRole), ErrorMessage = "Invalid teacher role value.")]
        public TeacherRole TeacherRole { get; set; }

        public AccountStatus Status { get; set; }
        public string? Phone { get; set; }
        [JsonIgnore]
        public int? SchoolId { get; set; }
        [JsonIgnore]
        public int? DepartmentId { get; set; }
    }
}
