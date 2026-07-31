using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntitySubscriptionsAndFinances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CampaignDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromotionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxRedemptions = table.Column<int>(type: "int", nullable: false),
                    RedemptionCount = table.Column<int>(type: "int", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_Campaigns_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityBillingProfiles",
                columns: table => new
                {
                    EntityBillingProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaxIdentificationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxCountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsVatNumberValidated = table.Column<bool>(type: "bit", nullable: false),
                    VatNumberValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BillingEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityBillingProfiles", x => x.EntityBillingProfileId);
                    table.ForeignKey(
                        name: "FK_EntityBillingProfiles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityBillingProfiles_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityBillingProfiles_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityBillingProfiles_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodTypes",
                columns: table => new
                {
                    PaymentMethodTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodTypes", x => x.PaymentMethodTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionDurationTypes",
                columns: table => new
                {
                    SubscriptionDurationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionDurationTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    AppliesPromo = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionDurationTypePromoPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionDurationTypes", x => x.SubscriptionDurationTypeId);
                    table.ForeignKey(
                        name: "FK_SubscriptionDurationTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionDurationTypes_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionDurationTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanTypes",
                columns: table => new
                {
                    SubscriptionPlanTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionPlanTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubscriptionPlanTypeDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanTypes", x => x.SubscriptionPlanTypeId);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTypes_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignLocalizations",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    CampaignNameDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampaignDescriptionDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignLocalizations", x => new { x.CampaignId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_CampaignLocalizations_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    GatewayCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodTypeId = table.Column<int>(type: "int", nullable: false),
                    LastFourDigits = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CardBrand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiryMonth = table.Column<int>(type: "int", nullable: true),
                    ExpiryYear = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodId);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_PaymentMethodTypes_PaymentMethodTypeId",
                        column: x => x.PaymentMethodTypeId,
                        principalTable: "PaymentMethodTypes",
                        principalColumn: "PaymentMethodTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodTypeCountries",
                columns: table => new
                {
                    PaymentMethodTypeId = table.Column<int>(type: "int", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodTypeCountries", x => new { x.PaymentMethodTypeId, x.CountryCode });
                    table.ForeignKey(
                        name: "FK_PaymentMethodTypeCountries_PaymentMethodTypes_PaymentMethodTypeId",
                        column: x => x.PaymentMethodTypeId,
                        principalTable: "PaymentMethodTypes",
                        principalColumn: "PaymentMethodTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodTypeLocalizations",
                columns: table => new
                {
                    PaymentMethodTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodTypeDisplayValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodTypeLocalizations", x => new { x.PaymentMethodTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_PaymentMethodTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentMethodTypeLocalizations_PaymentMethodTypes_PaymentMethodTypeId",
                        column: x => x.PaymentMethodTypeId,
                        principalTable: "PaymentMethodTypes",
                        principalColumn: "PaymentMethodTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionDurationTypeLocalizations",
                columns: table => new
                {
                    SubscriptionDurationTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionDurationTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionDurationTypeLocalizations", x => new { x.SubscriptionDurationTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_SubscriptionDurationTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionDurationTypeLocalizations_SubscriptionDurationTypes_SubscriptionDurationTypeId",
                        column: x => x.SubscriptionDurationTypeId,
                        principalTable: "SubscriptionDurationTypes",
                        principalColumn: "SubscriptionDurationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanDurationPrices",
                columns: table => new
                {
                    SubscriptionPlanDurationPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionPlanTypeId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionDurationTypeId = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToScale = table.Column<bool>(type: "bit", nullable: false),
                    ScaleRequirement = table.Column<int>(type: "int", nullable: false),
                    PricePerExtraMember = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncludedGenerations = table.Column<int>(type: "int", nullable: false),
                    PricePerExtraGeneration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPublicPlan = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanDurationPrices", x => x.SubscriptionPlanDurationPriceId);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanDurationPrices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanDurationPrices_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanDurationPrices_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanDurationPrices_SubscriptionDurationTypes_SubscriptionDurationTypeId",
                        column: x => x.SubscriptionDurationTypeId,
                        principalTable: "SubscriptionDurationTypes",
                        principalColumn: "SubscriptionDurationTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanDurationPrices_SubscriptionPlanTypes_SubscriptionPlanTypeId",
                        column: x => x.SubscriptionPlanTypeId,
                        principalTable: "SubscriptionPlanTypes",
                        principalColumn: "SubscriptionPlanTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlanTypeLocalizations",
                columns: table => new
                {
                    SubscriptionPlanTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlanTypeNameDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionPlanTypeDescriptionDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlanTypeLocalizations", x => new { x.SubscriptionPlanTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlanTypeLocalizations_SubscriptionPlanTypes_SubscriptionPlanTypeId",
                        column: x => x.SubscriptionPlanTypeId,
                        principalTable: "SubscriptionPlanTypes",
                        principalColumn: "SubscriptionPlanTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignSubscriptionPlans",
                columns: table => new
                {
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionPlanDurationPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignSubscriptionPlans", x => new { x.CampaignId, x.SubscriptionPlanDurationPriceId });
                    table.ForeignKey(
                        name: "FK_CampaignSubscriptionPlans_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignSubscriptionPlans_SubscriptionPlanDurationPrices_SubscriptionPlanDurationPriceId",
                        column: x => x.SubscriptionPlanDurationPriceId,
                        principalTable: "SubscriptionPlanDurationPrices",
                        principalColumn: "SubscriptionPlanDurationPriceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntitySubscriptionPlans",
                columns: table => new
                {
                    EntitySubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SubscriptionPlanDurationPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreviousSubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySubscriptionPlans", x => x.EntitySubscriptionPlanId);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPlans_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPlans_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPlans_EntitySubscriptionPlans_PreviousSubscriptionPlanId",
                        column: x => x.PreviousSubscriptionPlanId,
                        principalTable: "EntitySubscriptionPlans",
                        principalColumn: "EntitySubscriptionPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPlans_SubscriptionPlanDurationPrices_SubscriptionPlanDurationPriceId",
                        column: x => x.SubscriptionPlanDurationPriceId,
                        principalTable: "SubscriptionPlanDurationPrices",
                        principalColumn: "SubscriptionPlanDurationPriceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleGenerations",
                columns: table => new
                {
                    ScheduleGenerationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitySubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMs = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleGenerations", x => x.ScheduleGenerationId);
                    table.ForeignKey(
                        name: "FK_ScheduleGenerations_EntitySubscriptionPlans_EntitySubscriptionPlanId",
                        column: x => x.EntitySubscriptionPlanId,
                        principalTable: "EntitySubscriptionPlans",
                        principalColumn: "EntitySubscriptionPlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionBillingRecords",
                columns: table => new
                {
                    SubscriptionBillingRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitySubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UniqueMemberCountSnapshot = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExtraMembers = table.Column<int>(type: "int", nullable: false),
                    PricePerExtraMember = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncludedGenerations = table.Column<int>(type: "int", nullable: false),
                    GenerationsUsed = table.Column<int>(type: "int", nullable: false),
                    PricePerExtraGeneration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCharged = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionBillingRecords", x => x.SubscriptionBillingRecordId);
                    table.ForeignKey(
                        name: "FK_SubscriptionBillingRecords_EntitySubscriptionPlans_EntitySubscriptionPlanId",
                        column: x => x.EntitySubscriptionPlanId,
                        principalTable: "EntitySubscriptionPlans",
                        principalColumn: "EntitySubscriptionPlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntitySubscriptionPayments",
                columns: table => new
                {
                    EntitySubscriptionPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitySubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillingRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FailureCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FailureMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySubscriptionPayments", x => x.EntitySubscriptionPaymentId);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_EntitySubscriptionPlans_EntitySubscriptionPlanId",
                        column: x => x.EntitySubscriptionPlanId,
                        principalTable: "EntitySubscriptionPlans",
                        principalColumn: "EntitySubscriptionPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubscriptionPayments_SubscriptionBillingRecords_BillingRecordId",
                        column: x => x.BillingRecordId,
                        principalTable: "SubscriptionBillingRecords",
                        principalColumn: "SubscriptionBillingRecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionBillingRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityBillingProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ExternalProviderInvoiceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeriesCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ATCUD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    VatTreatment = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubtotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerLegalNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerTaxIdentificationNumberSnapshot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerTaxCountryCodeSnapshot = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerAddressLineSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerPostalCodeSnapshot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerCitySnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerCountryCodeSnapshot = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerTypeSnapshot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerCountryEvidence1 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerCountryEvidence1Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerCountryEvidence2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CustomerCountryEvidence2Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_EntityBillingProfiles_EntityBillingProfileId",
                        column: x => x.EntityBillingProfileId,
                        principalTable: "EntityBillingProfiles",
                        principalColumn: "EntityBillingProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_SubscriptionBillingRecords_SubscriptionBillingRecordId",
                        column: x => x.SubscriptionBillingRecordId,
                        principalTable: "SubscriptionBillingRecords",
                        principalColumn: "SubscriptionBillingRecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebhookEvents",
                columns: table => new
                {
                    WebhookEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitySubscriptionPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GatewayEventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookEvents", x => x.WebhookEventId);
                    table.ForeignKey(
                        name: "FK_PaymentWebhookEvents_EntitySubscriptionPayments_EntitySubscriptionPaymentId",
                        column: x => x.EntitySubscriptionPaymentId,
                        principalTable: "EntitySubscriptionPayments",
                        principalColumn: "EntitySubscriptionPaymentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignLocalizations_LocalizationId",
                table: "CampaignLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CreatedById",
                table: "Campaigns",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_DeletedById",
                table: "Campaigns",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_UpdatedById",
                table: "Campaigns",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignSubscriptionPlans_SubscriptionPlanDurationPriceId",
                table: "CampaignSubscriptionPlans",
                column: "SubscriptionPlanDurationPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityBillingProfiles_CreatedById",
                table: "EntityBillingProfiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityBillingProfiles_DeletedById",
                table: "EntityBillingProfiles",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityBillingProfiles_EntityId",
                table: "EntityBillingProfiles",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityBillingProfiles_UpdatedById",
                table: "EntityBillingProfiles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_BillingRecordId",
                table: "EntitySubscriptionPayments",
                column: "BillingRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_CreatedById",
                table: "EntitySubscriptionPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_DeletedById",
                table: "EntitySubscriptionPayments",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_EntitySubscriptionPlanId",
                table: "EntitySubscriptionPayments",
                column: "EntitySubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_PaymentMethodId",
                table: "EntitySubscriptionPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPayments_UpdatedById",
                table: "EntitySubscriptionPayments",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPlans_CampaignId",
                table: "EntitySubscriptionPlans",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPlans_EntityId",
                table: "EntitySubscriptionPlans",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPlans_EntityId_Status",
                table: "EntitySubscriptionPlans",
                columns: new[] { "EntityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPlans_PreviousSubscriptionPlanId",
                table: "EntitySubscriptionPlans",
                column: "PreviousSubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubscriptionPlans_SubscriptionPlanDurationPriceId",
                table: "EntitySubscriptionPlans",
                column: "SubscriptionPlanDurationPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CreatedById",
                table: "Invoices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DeletedById",
                table: "Invoices",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EntityBillingProfileId",
                table: "Invoices",
                column: "EntityBillingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SubscriptionBillingRecordId",
                table: "Invoices",
                column: "SubscriptionBillingRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UpdatedById",
                table: "Invoices",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CreatedById",
                table: "PaymentMethods",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_DeletedById",
                table: "PaymentMethods",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_EntityId",
                table: "PaymentMethods",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_PaymentMethodTypeId",
                table: "PaymentMethods",
                column: "PaymentMethodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UpdatedById",
                table: "PaymentMethods",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodTypeLocalizations_LocalizationId",
                table: "PaymentMethodTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookEvents_EntitySubscriptionPaymentId",
                table: "PaymentWebhookEvents",
                column: "EntitySubscriptionPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookEvents_GatewayEventId",
                table: "PaymentWebhookEvents",
                column: "GatewayEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleGenerations_EntitySubscriptionPlanId",
                table: "ScheduleGenerations",
                column: "EntitySubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBillingRecords_EntitySubscriptionPlanId",
                table: "SubscriptionBillingRecords",
                column: "EntitySubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionDurationTypeLocalizations_LocalizationId",
                table: "SubscriptionDurationTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionDurationTypes_CreatedById",
                table: "SubscriptionDurationTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionDurationTypes_DeletedById",
                table: "SubscriptionDurationTypes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionDurationTypes_UpdatedById",
                table: "SubscriptionDurationTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanDurationPrices_CreatedById",
                table: "SubscriptionPlanDurationPrices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanDurationPrices_DeletedById",
                table: "SubscriptionPlanDurationPrices",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanDurationPrices_PlanType_DurationType",
                table: "SubscriptionPlanDurationPrices",
                columns: new[] { "SubscriptionPlanTypeId", "SubscriptionDurationTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanDurationPrices_SubscriptionDurationTypeId",
                table: "SubscriptionPlanDurationPrices",
                column: "SubscriptionDurationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanDurationPrices_UpdatedById",
                table: "SubscriptionPlanDurationPrices",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanTypeLocalizations_LocalizationId",
                table: "SubscriptionPlanTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanTypes_CreatedById",
                table: "SubscriptionPlanTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanTypes_DeletedById",
                table: "SubscriptionPlanTypes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlanTypes_UpdatedById",
                table: "SubscriptionPlanTypes",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignLocalizations");

            migrationBuilder.DropTable(
                name: "CampaignSubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PaymentMethodTypeCountries");

            migrationBuilder.DropTable(
                name: "PaymentMethodTypeLocalizations");

            migrationBuilder.DropTable(
                name: "PaymentWebhookEvents");

            migrationBuilder.DropTable(
                name: "ScheduleGenerations");

            migrationBuilder.DropTable(
                name: "SubscriptionDurationTypeLocalizations");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanTypeLocalizations");

            migrationBuilder.DropTable(
                name: "EntityBillingProfiles");

            migrationBuilder.DropTable(
                name: "EntitySubscriptionPayments");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "SubscriptionBillingRecords");

            migrationBuilder.DropTable(
                name: "PaymentMethodTypes");

            migrationBuilder.DropTable(
                name: "EntitySubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanDurationPrices");

            migrationBuilder.DropTable(
                name: "SubscriptionDurationTypes");

            migrationBuilder.DropTable(
                name: "SubscriptionPlanTypes");
        }
    }
}
