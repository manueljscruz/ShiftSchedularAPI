using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Stores the payment method details associated with an entity, such as a user or organization, including card information and gateway identifiers.
    /// </summary>
    public class PaymentMethod : BaseEntity
    {
        /// <summary>
        /// Primary Key - The unique identifier for the payment method record.
        /// </summary>
        [Key]
        [Required]
        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated entity (user or organization) that owns this payment method. This links the payment method to a specific entity in the system.
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Identifier for the customer in the payment gateway's system. This ID is used to associate the payment method with the corresponding customer in the payment gateway, allowing for seamless transactions and management of payment methods.
        /// </summary>
        public string GatewayCustomerId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the type of payment method (e.g., credit card, debit card, PayPal, etc.). This links the payment method to a specific payment method type in the system.
        /// </summary>
        [Required]
        public int PaymentMethodTypeId { get; set; }

        /// <summary>
        /// Last four digits of the card number associated with the payment method. This information is used for display purposes and to help users identify their payment methods without exposing sensitive card information.
        /// </summary>
        [MaxLength(4)]
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Brand of the card associated with the payment method (e.g., Visa, MasterCard, American Express). This information is used for display purposes and to help users identify their payment methods.
        /// </summary>
        [MaxLength(50)]
        public string CardBrand { get; set; }

        /// <summary>
        /// Expiry month of the card associated with the payment method. This information is used for display purposes and to help users identify their payment methods.
        /// </summary>
        [Range(1, 12)]
        public int? ExpiryMonth { get; set; }

        /// <summary>
        /// Expiry year of the card associated with the payment method. This information is used for display purposes and to help users identify their payment methods.
        /// </summary>
        [Range(1900, 2100)]
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Token provided by the payment gateway for the payment method. This token is used for secure transactions and to avoid storing sensitive card information directly in the system.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Indicates whether the payment method is the default payment method for the associated entity. If true, this payment method will be used as the primary method for transactions.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Indicates whether the payment method is active or not. If false, it means the payment method is inactive and cannot be used for transactions.
        /// </summary>
        public bool IsActive { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity associated with this payment method, providing context for the ownership and usage of the payment method.
        /// </summary>
        public virtual Entity Entity { get; set; }

        /// <summary>
        /// Payment method type entity associated with this payment method, providing context for the type of payment method (e.g., credit card, debit card, PayPal, etc.) and its characteristics.
        /// </summary>
        public virtual PaymentMethodType PaymentMethodType { get; set; }

        /// <summary>
        /// Collection of payment attempts made using this payment method.
        /// </summary>
        public virtual ICollection<EntitySubscriptionPayment> EntitySubscriptionPayments { get; set; }

        #endregion
    }
}
