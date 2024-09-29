using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Validations
{
    public class ValidDateOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateString = value.ToString();

            // Try to parse the string to DateOnly
            if (!DateOnly.TryParse(dateString, out _))
            {
                return new ValidationResult(ErrorMessage ?? "Invalid date format.");
            }

            return ValidationResult.Success;
        }
    }
}
