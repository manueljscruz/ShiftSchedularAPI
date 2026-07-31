using ShiftSchedularEntity.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Invoice entity represents a fiscal document issued (or pending issuance) for a subscription billing record. It stores an immutable snapshot of the customer's fiscal data at the time of issuance, the applied VAT treatment, and the identifiers returned by the external certified invoicing provider (e.g., Primavera, Moloni) once the document has been generated there.
    /// </summary>
    public class Invoice : BaseEntity
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Invoice.
        /// </summary>
        [Key]
        [Required]
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the subscription billing record that originated this invoice.
        /// </summary>
        [Required]
        public Guid SubscriptionBillingRecordId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the billing profile the customer data snapshot was copied from at the time of issuance. Nullable — kept only for traceability, since the snapshot fields below are self-contained.
        /// </summary>
        public Guid? EntityBillingProfileId { get; set; }

        /// <summary>
        /// Status of the invoice (e.g., Pending, Issued, Failed, Cancelled, Credited).
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Status { get; set; }

        /// <summary>
        /// Identifier of the document as assigned by the external certified invoicing provider. Null until the provider integration issues the document.
        /// </summary>
        [MaxLength(100)]
        public string ExternalProviderInvoiceId { get; set; }

        /// <summary>
        /// Invoice series code assigned by the external provider (e.g., "A/2026"). Null until issued.
        /// </summary>
        [MaxLength(50)]
        public string SeriesCode { get; set; }

        /// <summary>
        /// Sequential document number within the series, assigned by the external provider. Null until issued.
        /// </summary>
        [MaxLength(50)]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// ATCUD (Código Único do Documento) assigned by the external certified provider. Null until issued.
        /// </summary>
        [MaxLength(50)]
        public string ATCUD { get; set; }

        /// <summary>
        /// Link/reference to the generated document (e.g., PDF URL), if returned by the external provider.
        /// </summary>
        [MaxLength(500)]
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Date and time the invoice was issued by the external provider. Null while the invoice is still pending.
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// ISO 4217 currency code for the amounts on this invoice (e.g., EUR).
        /// </summary>
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }

        /// <summary>
        /// VAT treatment applied to this invoice (e.g., National, ReverseCharge, OutOfScope, DestinationCountryVat).
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string VatTreatment { get; set; }

        /// <summary>
        /// VAT rate (percentage) applied to this invoice. Null when VatTreatment is ReverseCharge or OutOfScope.
        /// </summary>
        public decimal? VatRate { get; set; }

        /// <summary>
        /// VAT amount calculated for this invoice.
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// Amount before VAT.
        /// </summary>
        public decimal SubtotalAmount { get; set; }

        /// <summary>
        /// Total amount charged, including VAT.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Snapshot of the customer's legal name at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CustomerLegalNameSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer's tax identification number at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CustomerTaxIdentificationNumberSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the country code of the customer's tax identification number at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string CustomerTaxCountryCodeSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer's fiscal address line at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CustomerAddressLineSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer's fiscal postal code at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string CustomerPostalCodeSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer's fiscal city at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CustomerCitySnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer's fiscal address country code at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string CustomerCountryCodeSnapshot { get; set; }

        /// <summary>
        /// Snapshot of the customer type (Organization/Individual) at the time of issuance.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string CustomerTypeSnapshot { get; set; }

        /// <summary>
        /// First piece of non-contradictory evidence of the customer's location, as required for B2C EU digital services (e.g., a country code). Only relevant for B2C EU invoices.
        /// </summary>
        [MaxLength(2)]
        public string CustomerCountryEvidence1 { get; set; }

        /// <summary>
        /// Source that produced the first location evidence (e.g., "BillingAddress").
        /// </summary>
        [MaxLength(50)]
        public string CustomerCountryEvidence1Source { get; set; }

        /// <summary>
        /// Second piece of non-contradictory evidence of the customer's location, as required for B2C EU digital services (e.g., a country code). Only relevant for B2C EU invoices.
        /// </summary>
        [MaxLength(2)]
        public string CustomerCountryEvidence2 { get; set; }

        /// <summary>
        /// Source that produced the second location evidence (e.g., "PaymentMethod").
        /// </summary>
        [MaxLength(50)]
        public string CustomerCountryEvidence2Source { get; set; }

        /// <summary>
        /// Reason the invoice failed to be issued, populated when Status = Failed.
        /// </summary>
        [MaxLength(500)]
        public string FailureReason { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Subscription billing record that originated this invoice, providing context for the charged amount and period.
        /// </summary>
        public virtual SubscriptionBillingRecord SubscriptionBillingRecord { get; set; }

        /// <summary>
        /// Billing profile the customer data snapshot on this invoice was copied from, if known.
        /// </summary>
        public virtual EntityBillingProfile? EntityBillingProfile { get; set; }

        #endregion
    }
}
