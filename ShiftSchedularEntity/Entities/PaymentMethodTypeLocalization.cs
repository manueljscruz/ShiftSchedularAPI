using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Localization entity represents the localized display value of a Payment Method Type for a specific language or culture. It contains information about the Payment Method Type, the Localization, and the localized display value.
    /// </summary>
    public class PaymentMethodTypeLocalization
    {
        /// <summary>
        /// Composite Key
        /// Table of Origin - Payment Method Type
        /// </summary>
        
        [Required]
        public int PaymentMethodTypeId { get; set; }

        /// <summary>
        /// Composite Key
        /// Table of Origin - Localization
        /// </summary>
        
        [Required]
        public int LocalizationId { get; set; }

        /// <summary>
        /// Localized display value of Payment Method
        /// </summary>
        [MaxLength(50)]
        public string PaymentMethodTypeDisplayValue { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Localization entity associated with the Payment Method Type Localization. This property represents the relationship between the Payment Method Type Localization and the Localization entity, allowing access to the details of the specific localization (language or culture) for which the display value is defined.
        /// </summary>
        public virtual Localization Localization { get; set; }

        /// <summary>
        /// Payment Method Type entity associated with the Payment Method Type Localization. This property represents the relationship between the Payment Method Type Localization and the Payment Method Type entity, allowing access to the details of the specific payment method type for which the display value is defined.
        /// </summary>
        public virtual PaymentMethodType PaymentMethodType { get; set; }

        #endregion
    }
}
