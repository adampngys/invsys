using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.Common
{
    public class MyRangeAttribute : ValidationAttribute
    {
        private readonly string _maxPropertyName;
        public MyRangeAttribute(string maxPropertyName)
        {
            _maxPropertyName = maxPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maxProperty = validationContext.ObjectType.GetProperty(_maxPropertyName);
            if (maxProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _maxPropertyName));
            }

            int minValue = 0;
            int maxValue = (int)maxProperty.GetValue(validationContext.ObjectInstance, null);
            int currentValue = (int)value;
            if (currentValue < minValue || currentValue > maxValue)
            {
                return new ValidationResult(
                    string.Format(
                        ErrorMessage,
                        minValue,
                        maxValue
                    )
                );
            }

            return null;
        }
    }
}