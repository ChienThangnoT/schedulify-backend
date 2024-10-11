using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class CreateSchoolManagerModel
    {
        public int SchoolId { get; set; }
        [Required(ErrorMessage = "Email is required!"), EmailAddress(ErrorMessage = "Please enter valid email!")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Account phone is required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone must be exactly 10 characters.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public required string Phone { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string ConfirmAccountPassword { get; set; }
        [JsonIgnore]
        public bool IsChangeDefaultPassword { get; set; }= true;
        [JsonIgnore]
        public int Status { get; set; } = (int)AccountStatus.Pending;
    }

    public class CreateAdmin    
    {
        [Required(ErrorMessage = "Email is required!"), EmailAddress(ErrorMessage = "Please enter valid email!")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Account phone is required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone must be exactly 10 characters.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public required string Phone { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string ConfirmAccountPassword { get; set; }
        [JsonIgnore]
        public bool IsChangeDefaultPassword { get; set; } = true;
        [JsonIgnore]
        public int Status { get; set; } = (int)AccountStatus.Pending;
    }
}
