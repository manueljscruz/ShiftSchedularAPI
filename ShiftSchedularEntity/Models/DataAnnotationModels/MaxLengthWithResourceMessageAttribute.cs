using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace ShiftSchedularEntity.Models.DataAnnotationModels
{
    public class MaxLengthWithResourceMessageAttribute : MaxLengthAttribute
    {
        public MaxLengthWithResourceMessageAttribute(int length, Type resourceType, string resourceKey)
            : base(length)
        {
            if (resourceType == null) throw new ArgumentNullException(nameof(resourceType));
            if (string.IsNullOrWhiteSpace(resourceKey)) throw new ArgumentNullException(nameof(resourceKey));

            // Use reflection to get the ResourceManager property
            var resourceManagerProperty = resourceType.GetProperty("ResourceManager");
            if (resourceManagerProperty == null)
                throw new ArgumentException("The provided resourceType does not have a ResourceManager property.");

            var resourceManager = (ResourceManager)resourceManagerProperty.GetValue(null);

            // Fetch the error message from the resource file
            ErrorMessage = resourceManager.GetString(resourceKey);

            if (ErrorMessage == null)
                throw new ArgumentException($"The key '{resourceKey}' was not found in the resource file.");
        }
    }
}
