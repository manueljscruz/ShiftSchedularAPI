using ShiftSchedularEntity.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Entity Billing Profile entity stores the fiscal identity of an entity for invoicing purposes — legal name, tax identification number, fiscal address and customer classification. This data changes infrequently and is snapshotted onto each Invoice at the time it is issued, so later changes here never affect already-issued invoices.
    /// </summary>
    public class EntityBillingProfile : BaseEntity
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Entity Billing Profile.
        /// </summary>
        [Key]
        [Required]
        public Guid EntityBillingProfileId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the associated entity that this billing profile belongs to.
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Legal name / denominação social of the entity for fiscal purposes. May differ from Entity.EntityName, which can be a more casual display name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string LegalName { get; set; }

        /// <summary>
        /// Tax identification number (e.g., NIF in Portugal, VAT number elsewhere). Generic field — not assumed to always be Portuguese.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TaxIdentificationNumber { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of the tax identification number (e.g., PT, DE).
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string TaxCountryCode { get; set; }

        /// <summary>
        /// Street/address line of the fiscal address.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string AddressLine { get; set; }

        /// <summary>
        /// Postal code of the fiscal address.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        /// <summary>
        /// City of the fiscal address.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of the fiscal address.
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Classifies the customer as an organization (B2B) or an individual (B2C). Determines which VAT treatment rules apply when an invoice is generated.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string CustomerType { get; set; }

        /// <summary>
        /// Indicates whether the tax identification number has been validated against VIES. Required before reverse charge can be applied to B2B EU customers.
        /// </summary>
        public bool IsVatNumberValidated { get; set; }

        /// <summary>
        /// Date and time when the tax identification number was last validated against VIES. Null if never validated.
        /// </summary>
        public DateTime? VatNumberValidatedAt { get; set; }

        /// <summary>
        /// Email address invoices should be sent to, if different from the entity's login/contact email.
        /// </summary>
        [MaxLength(200)]
        public string BillingEmail { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity associated with this billing profile.
        /// </summary>
        public virtual Entity Entity { get; set; }

        /// <summary>
        /// Collection of invoices whose customer data snapshot originated from this billing profile.
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; }

        #endregion
    }
}
