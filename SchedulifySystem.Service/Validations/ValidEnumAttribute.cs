using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Validations
{
    public class ValidEnumAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an Enum.");
            }

            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} is required.");
            }

            // Kiểm tra nếu giá trị là kiểu chuỗi
            if (value is string stringValue)
            {
                // Kiểm tra giá trị có tồn tại trong enum không
                if (Enum.TryParse(_enumType, stringValue, true, out var _))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"Invalid value '{stringValue}' for enum '{_enumType.Name}'.");
            }

            // Kiểm tra nếu giá trị là kiểu số
            if (value is int intValue)
            {
                if (Enum.IsDefined(_enumType, intValue))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"Invalid value '{intValue}' for enum '{_enumType.Name}'.");
            }

            return new ValidationResult($"Invalid value for enum '{_enumType.Name}'.");
        }
    }
}
