using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription plans associated with entities, including details about the subscription plan duration, pricing, and any associated campaigns or previous subscription plans.
    /// </summary>
    public class EntitySubscriptionPlan
    {
        /// <summary>
        /// Primary Key - The unique identifier for the entity subscription plan.
        /// </summary>
        [Key]
        [Required]
        public Guid EntitySubscriptionPlanId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated entity. This links the subscription plan to a specific entity that is subscribed to the plan.
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the subscription plan duration price. This links the subscription plan to a specific duration and price combination.
        /// </summary>
        [Required]
        public Guid SubscriptionPlanDurationPriceId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated campaign, if applicable. This links the subscription plan to a specific campaign that may offer special pricing or benefits.
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the previous subscription plan, if applicable. This links the subscription plan to a previous plan that the entity was subscribed to.
        /// </summary>
        public Guid? PreviousSubscriptionPlanId { get; set; }

        /// <summary>
        /// Start date of the subscription plan, indicating when the subscription becomes active for the entity.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the subscription plan, indicating when the subscription ends for the entity. This can be null if the subscription is ongoing or has no specific end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Status of the subscription plan, indicating whether it is active, expired, canceled, or any other relevant status. This field helps in tracking the current state of the subscription plan for the entity.
        /// </summary>
        public string Status { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity associated with this subscription plan, providing context for the subscription plan's ownership and usage.
        /// </summary>
        public virtual Entity Entity { get; set; }

        /// <summary>
        /// Campaign associated with this subscription plan, providing context for any special pricing or benefits that may be applied to the subscription plan due to the campaign.
        /// </summary>
        public virtual Campaign? Campaign { get; set; }

        /// <summary>
        /// Subscription plan duration price associated with this subscription plan, indicating the specific duration and price combination that applies to the entity subscription plan.
        /// </summary>
        public virtual SubscriptionPlanDurationPrice SubscriptionPlanDurationPrice { get; set; }

        /// <summary>
        /// Previous subscription plan associated with this subscription plan, providing context for any prior subscription plans that the entity was subscribed to before the current plan.
        /// </summary>
        public virtual EntitySubscriptionPlan? PreviousSubscriptionPlan { get; set; }

        /// <summary>
        /// Collection of Schedule Generations associated with this subscription plan, providing context for any scheduling or generation-related activities that may be linked to the entity subscription plan.
        /// </summary>
        public virtual ICollection<ScheduleGeneration> ScheduleGenerations { get; set; }

        /// <summary>
        /// Collection of payment attempts made for this subscription plan.
        /// </summary>
        public virtual ICollection<EntitySubscriptionPayment> EntitySubscriptionPayments { get; set; }

        /// <summary>
        /// Collection of billing records generated for this subscription plan. Expected to contain at most one record per billing period.
        /// </summary>
        public virtual ICollection<SubscriptionBillingRecord> SubscriptionBillingRecords { get; set; }

        #endregion
    }
}
