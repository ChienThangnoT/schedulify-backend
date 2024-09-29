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
        [JsonIgnore]
        public string? FirstName { get; set; }
        [JsonIgnore]
        public string? LastName { get; set; }
        public int SchoolId { get; set; }
        [Required(ErrorMessage = "Email is required!"), EmailAddress(ErrorMessage = "Please enter valid email!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation does not match!")]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public string ConfirmAccountPassword { get; set; }
        [JsonIgnore]
        public bool IsChangeDefaultPassword { get; set; }= true;
        [JsonIgnore]
        public int Status { get; set; } = (int)AccountStatus.Active;
        [JsonIgnore]
        public int Phone { get; set; }
        [JsonIgnore]
        public string AvatarURL { get; set; } = string.Empty;
    }
}
