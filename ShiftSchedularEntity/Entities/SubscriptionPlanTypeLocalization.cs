using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Plan Type Localization entity represents the localized display values for different subscription plan types. It contains information about the subscription plan type, the associated localization, and the localized display values for the subscription plan name and description.
    /// </summary>
    public class SubscriptionPlanTypeLocalization
    {
        /// <summary>
        /// Composite Key 1 - Unique identifier for the associated Subscription Plan Type. This property establishes a relationship between the Subscription Plan Type Localization and the corresponding subscription plan type, allowing access to the details of the subscription plan.
        /// </summary>
        [Required]
        public int SubscriptionPlanTypeId { get; set; }

        /// <summary>
        /// Composite Key 2 - Unique identifier for the associated Localization. This property establishes a relationship between the Subscription Plan Type Localization and the corresponding localization, allowing access to the details of the localization.
        /// </summary>
        [Required]
        public int LocalizationId { get; set; }

        /// <summary>
        /// Subscription Plan Type Name Display Value - The localized display value for the subscription plan type name. This property provides the translated or region-specific name for the subscription plan type, allowing the application to present the appropriate display value based on the user's language or locale.
        /// </summary>
        public string SubscriptionPlanTypeNameDisplayValue { get; set; }

        /// <summary>
        /// Subscription Plan Type Description Display Value - The localized display value for the subscription plan type description. This property provides the translated or region-specific description for the subscription plan type, allowing the application to present the appropriate display value based on the user's language or locale.
        /// </summary>
        public string SubscriptionPlanTypeDescriptionDisplayValue { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Localization entity associated with this Subscription Plan Type Localization. This navigation property allows access to the details of the localization, including language, region, and other relevant information.
        /// </summary>
        public virtual Localization Localization { get; set; }

        /// <summary>
        /// Subscription Plan Type entity associated with this Subscription Plan Type Localization. This navigation property allows access to the details of the subscription plan type, including its name, description, and other relevant information.
        /// </summary>
        public virtual SubscriptionPlanType SubscriptionPlanType { get; set; }

        #endregion
    }
}
