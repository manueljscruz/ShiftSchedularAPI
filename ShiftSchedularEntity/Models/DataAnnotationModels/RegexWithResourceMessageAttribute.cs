using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Text.RegularExpressions;

namespace ShiftSchedularEntity.Models.DataAnnotationModels
{
    public class RegexWithResourceMessageAttribute : ValidationAttribute
    {
        private readonly string _pattern;

        public RegexWithResourceMessageAttribute(string pattern, Type resourceType, string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentNullException(nameof(pattern));
            if (resourceType == null) throw new ArgumentNullException(nameof(resourceType));
            if (string.IsNullOrWhiteSpace(resourceKey)) throw new ArgumentNullException(nameof(resourceKey));

            _pattern = pattern;

            // Fetch error message from resource file
            var resourceManagerProperty = resourceType.GetProperty("ResourceManager");
            if (resourceManagerProperty == null)
                throw new ArgumentException("The provided resourceType does not have a ResourceManager property.");

            var resourceManager = (ResourceManager)resourceManagerProperty.GetValue(null);
            ErrorMessage = resourceManager.GetString(resourceKey);

            if (ErrorMessage == null)
                throw new ArgumentException($"The key '{resourceKey}' was not found in the resource file.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var input = value.ToString();
            if (Regex.IsMatch(input, _pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
