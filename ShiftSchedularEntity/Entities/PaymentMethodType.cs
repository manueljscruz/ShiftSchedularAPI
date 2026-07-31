using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Payment method type entity represents the different types of payment methods available in the system, such as Card, MBWay, Paypal, etc. This entity allows for the categorization and management of various payment methods, enabling users to select their preferred payment option during transactions.
    /// </summary>
    public class PaymentMethodType
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Payment Method Type.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentMethodTypeId { get; set; }

        /// <summary>
        /// Payment Method Type Name (Card, MBWay, Paypal, etc)
        /// </summary>
        public string PaymentMethodTypeName { get; set; }

        /// <summary>
        /// Flag indicating if this Payment method is active
        /// </summary>
        public bool IsActive { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Collection of PaymentMethodTypeCountry entities associated with this Payment Method Type, representing the countries where this payment method is available.
        /// </summary>
        public virtual ICollection<PaymentMethodTypeCountry> PaymentMethodTypeCountries { get; set; }

        /// <summary>
        /// Collection of PaymentMethodTypeLocalization entities associated with this Payment Method Type, representing the localized display values for this payment method in different languages and regions.
        /// </summary>
        public virtual ICollection<PaymentMethodTypeLocalization> PaymentMethodTypeLocalizations { get; set; }

        /// <summary>
        /// Collection of PaymentMethod entities that use this Payment Method Type.
        /// </summary>
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }

        #endregion
    }
}
