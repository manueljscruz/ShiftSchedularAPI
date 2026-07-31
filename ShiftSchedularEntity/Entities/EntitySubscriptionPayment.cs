using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Stores all the attempts successful or failed to charge a subscription plan payment for an entity. This entity captures the details of each payment attempt, including the associated subscription plan, billing record, payment method, transaction ID, status, and any failure information.
    /// </summary>
    public class EntitySubscriptionPayment : BaseEntity
    {
        /// <summary>
        /// Primary Key - The unique identifier for the entity subscription payment record.
        /// </summary>
        [Key]
        [Required]
        public Guid EntitySubscriptionPaymentId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated entity subscription plan. This links the payment attempt to a specific subscription plan that the entity is subscribed to.
        /// </summary>
        [Required]
        public Guid EntitySubscriptionPlanId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated subscription billing record. This links the payment attempt to a specific billing record that contains details about the billing cycle and amount due.
        /// </summary>
        [Required]
        public Guid BillingRecordId { get; set; }
        
        /// <summary>
        /// Foreign Key - The unique identifier for the associated payment method. This links the payment attempt to a specific payment method used for the transaction.
        /// </summary>
        [Required]
        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Identifier for the transaction as provided by the payment gateway. This ID is used to track and reference the specific transaction in the payment gateway's system.
        /// </summary>
        [MaxLength(100)]
        public string GatewayTransactionId { get; set; }

        /// <summary>
        /// Status of the payment attempt, indicating whether it was successful, failed, pending, or any other relevant status. This field helps in tracking the outcome of the payment attempt.
        /// </summary>
        [MaxLength(30)]
        public string Status { get; set; }

        /// <summary>
        /// Failure code returned by the payment gateway in case of a failed payment attempt. This code helps in identifying the reason for the failure and can be used for troubleshooting and reporting purposes.
        /// </summary>
        [MaxLength(50)]
        public string FailureCode { get; set; }

        /// <summary>
        /// Failure message returned by the payment gateway in case of a failed payment attempt. This message provides additional context and details about the failure, which can be useful for understanding the issue and communicating it to the user or support team.
        /// </summary>
        [MaxLength(500)]
        public string FailureMessage { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity subscription plan entity associated with this payment attempt, providing context for the subscription plan that the entity is subscribed to and for which the payment was attempted.
        /// </summary>
        public virtual EntitySubscriptionPlan EntitySubscriptionPlan { get; set; }

        /// <summary>
        /// Subscription billing record entity associated with this payment attempt, providing context for the billing cycle and amount due that the payment was intended to cover.
        /// </summary>
        public virtual SubscriptionBillingRecord BillingRecord { get; set; }

        /// <summary>
        /// Payment method entity associated with this payment attempt, providing context for the payment method used for the transaction.
        /// </summary>
        public virtual PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Collection of webhook events received from the payment gateway related to this payment attempt.
        /// </summary>
        public virtual ICollection<PaymentWebhookEvent> PaymentWebhookEvents { get; set; }

        #endregion
    }
}
