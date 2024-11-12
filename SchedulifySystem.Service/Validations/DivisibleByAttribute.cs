using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Validations
{
    public class DivisibleByAttribute : ValidationAttribute
    {
        private readonly int _divisor;

        public DivisibleByAttribute(int divisor)
        {
            _divisor = divisor;
            ErrorMessage = $"The value must be divisible by {_divisor}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (int)value % _divisor != 0)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
