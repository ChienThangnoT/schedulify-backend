using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.AccountBusinessModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Password is required!")]
        [Display(Name = "OldPassword")]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "Password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string OldPasswoorrdPassword { get; set; }
        [Required(ErrorMessage = "New Password is required!")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "New password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation does not match!")]
        [StringLength(12, MinimumLength = 7, ErrorMessage = "New password must be 7-12 Character")]
        [PasswordPropertyText]
        public required string ConfirmNewPassword { get; set; }
    }
}
