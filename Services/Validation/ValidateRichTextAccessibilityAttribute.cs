using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Services.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateRichTextAccessibilityAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var richText = value as string;

            if (string.IsNullOrWhiteSpace(richText))
                return ValidationResult.Success;

            var validator = (IRichTextValidator)validationContext.GetService(typeof(IRichTextValidator))!;
            List<string> errors = validator.Validate(richText);

            if (errors.Count == 0)
                return ValidationResult.Success;

            string combined = string.Join(" ", errors);
            return new ValidationResult(combined);
        }
    }
}
    