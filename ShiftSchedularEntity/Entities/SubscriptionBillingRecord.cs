using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Billing Record entity represents a record of billing information for a specific subscription plan. It contains details about the billing period, member counts, pricing, and total charges associated with the subscription.
    /// </summary>
    public class SubscriptionBillingRecord
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Subscription Billing Record
        /// </summary>
        [Key]
        [Required]
        public Guid SubscriptionBillingRecordId { get; set; }

        /// <summary>
        /// Foreign Key - Identifier of the associated Entity Subscription Plan
        /// </summary>
        [Required]
        public Guid EntitySubscriptionPlanId { get; set; }

        /// <summary>
        /// Start date of the billing period. This is the date when the billing period begins, and it is used to calculate charges for the subscription during this time frame.
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// End date of the billing period. This is the date when the billing period ends, and it is used to calculate charges for the subscription during this time frame.
        /// </summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Unique count of members associated with the subscription plan at the time of billing. This value is used to determine the base price and any additional charges for extra members.
        /// </summary>
        public int UniqueMemberCountSnapshot { get; set; }

        /// <summary>
        /// Base price of the subscription plan for the billing period. This value represents the standard cost of the subscription without any additional charges for extra members or generations.
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Number of extra members beyond the included members in the subscription plan.
        /// </summary>
        public int ExtraMembers { get; set; }

        /// <summary>
        /// Price charged per extra member beyond the included members in the subscription plan. This value is used to calculate additional charges for any extra members associated with the subscription during the billing period.
        /// </summary>
        public decimal PricePerExtraMember { get; set; }

        /// <summary>
        /// Number amount of generations included in the subscription plan. This value represents the number of generations that are covered by the base price of the subscription, and any additional generations beyond this limit may incur extra charges.
        /// </summary>
        public int IncludedGenerations { get; set; }

        /// <summary>
        /// Number amount of generations used during the billing period. This value is used to determine if any extra charges are applicable for generations beyond the included limit in the subscription plan.
        /// </summary>
        public int GenerationsUsed { get; set; }

        /// <summary>
        /// Price charged per extra generation beyond the included generations in the subscription plan. This value is used to calculate additional charges for any extra generations used during the billing period.
        /// </summary>
        public decimal PricePerExtraGeneration { get; set; }

        /// <summary>
        /// Total amount charged for the subscription during the billing period. This value includes the base price, charges for extra members, and charges for extra generations.
        /// </summary>
        public decimal TotalCharged { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity Subscription Plan associated with this billing record. This navigation property allows access to the details of the subscription plan for which this billing record was generated.
        /// </summary>
        public virtual EntitySubscriptionPlan EntitySubscriptionPlan { get; set; }

        /// <summary>
        /// Collection of Entity Subscription Payments associated with this billing record. This navigation property allows access to the payment records related to this billing record, enabling tracking of payments made for the subscription during the billing period.
        /// </summary>
        public virtual ICollection<EntitySubscriptionPayment> EntitySubscriptionPayments { get; set; }

        /// <summary>
        /// Collection of invoices issued (or pending issuance) for this billing record. Normally a single invoice, with additional entries only for corrections such as credit notes.
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; }

        #endregion
    }
}
