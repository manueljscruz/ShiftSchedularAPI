using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Payment Webhook Event entity represents an event received from a payment gateway related to a subscription payment. It contains information about the event, including the associated subscription payment, the type of event, timestamps for when the event was received and processed, and the raw payload of the event.
    /// </summary>
    public class PaymentWebhookEvent
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Payment Webhook Event.
        /// </summary>
        [Key]
        [Required]
        public Guid WebhookEventId { get; set; }

        /// <summary>
        /// Foreign Key - Unique identifier for the associated Entity Subscription Payment. This property establishes a relationship between the Payment Webhook Event and the corresponding subscription payment, allowing access to the details of the payment that triggered the webhook event.
        /// </summary>
        public Guid? EntitySubscriptionPaymentId { get; set; }

        /// <summary>
        /// Gateway Event ID - Unique identifier for the event as provided by the payment gateway. This property allows tracking and referencing the specific event received from the payment gateway, facilitating troubleshooting and reconciliation of events.
        /// </summary>
        public string GatewayEventId { get; set; }

        /// <summary>
        /// Event type - Describes the type of event received from the payment gateway (e.g., payment succeeded, payment failed, subscription canceled). This property provides context about the nature of the event, enabling appropriate handling and processing of different event types.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Received At - Timestamp indicating when the webhook event was received by the system. This property helps track the timing of events and can be useful for auditing and monitoring purposes.
        /// </summary>
        public DateTime ReceivedAt { get; set; }

        /// <summary>
        /// Processed At - Timestamp indicating when the webhook event was processed by the system. This property helps track the processing time and can be useful for auditing and monitoring purposes.
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Raw Payload - The raw JSON payload received from the payment gateway. This property allows storing the complete event data for auditing, debugging, and processing purposes.
        /// </summary>
        public string RawPayload { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Navigation property for the associated Entity Subscription Payment. This property allows access to the details of the subscription payment related to the webhook event.
        /// </summary>
        public virtual EntitySubscriptionPayment? EntitySubscriptionPayment { get; set; }

        #endregion
    }
}
