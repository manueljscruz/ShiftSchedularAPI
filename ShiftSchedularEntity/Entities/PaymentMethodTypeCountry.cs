using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Payment Method Type Country entity represents the association between a payment method type and the countries where it is available. This entity allows for the specification of which payment methods can be used in specific countries, enabling localized payment options for users based on their geographic location.
    /// </summary>
    public class PaymentMethodTypeCountry
    {
        /// <summary>
        /// Composite Key 1 - Unique identifier for the Payment Method Type, linking this entity to the corresponding payment method type in the system.
        /// </summary>

        [Required]
        public int PaymentMethodTypeId { get; set; }

        /// <summary>
        /// Country Code to where this payment method type is available
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Payment Method Type entity associated with this PaymentMethodTypeCountry, providing context for the payment method type and its availability in specific countries.
        /// </summary>
        public virtual PaymentMethodType PaymentMethodType { get; set; }

        #endregion
    }
}
