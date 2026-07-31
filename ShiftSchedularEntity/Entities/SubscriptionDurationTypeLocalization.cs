using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Duration Type Localization entity represents the localized display values for different subscription duration types. It contains information about the subscription duration type, the associated localization, and the localized display value for that duration type.
    /// </summary>
    public class SubscriptionDurationTypeLocalization
    {
        /// <summary>
        /// Composite Key 1 - Unique identifier for the associated Subscription Duration Type. This property establishes a relationship between the Subscription Duration Type Localization and the corresponding subscription duration type, allowing access to the details of the duration type.
        /// </summary>
        [Required]
        public int SubscriptionDurationTypeId { get; set; }

        /// <summary>
        /// Composite Key 2 - Unique identifier for the associated Localization. This property establishes a relationship between the Subscription Duration Type Localization and the corresponding localization, allowing access to the details of the localization.
        /// </summary>
        [Required]
        public int LocalizationId { get; set; }

        /// <summary>
        /// Localized Display Value - The localized display value for the subscription duration type. This property provides the translated or region-specific name for the duration type, allowing the application to present the appropriate display value based on the user's language or locale.
        /// </summary>
        public string SubscriptionDurationTypeDisplayValue { get; set; }


        #region Navigation Properties

        /// <summary>
        /// Localization entity associated with this Subscription Duration Type Localization. This navigation property allows access to the details of the localization, including language, region, and other relevant information.
        /// </summary>
        public virtual Localization Localization { get; set; }

        /// <summary>
        /// Subscription Duration Type entity associated with this Subscription Duration Type Localization. This navigation property allows access to the details of the subscription duration type, including its name, duration, and promotional information.
        /// </summary>
        public virtual SubscriptionDurationType SubscriptionDurationType { get; set; }

        #endregion
    }
}
