using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Campaign localization entity that stores localized display values for campaign names and descriptions.
    /// </summary>
    public class CampaignLocalization
    {
        /// <summary>
        /// Composite Key 1 - The unique identifier for the campaign. 
        /// </summary>
        [Required]
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Composite Key 2 - The unique identifier for the localization.
        /// </summary>
        [Required]
        public int LocalizationId { get; set; }

        /// <summary>
        /// Localized display value for the campaign name.
        /// </summary>
        public string CampaignNameDisplayValue { get; set; }

        /// <summary>
        /// Localized display value for the campaign description.
        /// </summary>
        public string CampaignDescriptionDisplayValue { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Localization entity associated with this campaign localization, providing language and regional context.
        /// </summary>
        public virtual Localization Localization { get; set; }

        /// <summary>
        /// Campaign entity associated with this campaign localization, providing context for the localized content.
        /// </summary>
        public virtual Campaign Campaign { get; set; }

        #endregion
    }
}
