using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class DbRebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsenceTypes",
                columns: table => new
                {
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AbsenceTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceTypes", x => x.AbsenceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessAspects",
                columns: table => new
                {
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessAspectName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAspects", x => x.BusinessAspectId);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTypeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.EntityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenderValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "Localizations",
                columns: table => new
                {
                    LocalizationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalizationCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizations", x => x.LocalizationId);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypes",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MultipleSpecification = table.Column<bool>(type: "bit", nullable: false),
                    IsSpecValuesBoolean = table.Column<bool>(type: "bit", nullable: false),
                    RuleTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypes", x => x.RuleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreakTypes",
                columns: table => new
                {
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftBreakTypeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTypes", x => x.ShiftBreakTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplates",
                columns: table => new
                {
                    ShiftTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftStartHour = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    TemplateClicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplates", x => x.ShiftTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HexBGColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HexFontColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Entities_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "GenderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbsenceTypeLocalizations",
                columns: table => new
                {
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    AbsenceTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceTypeLocalizations", x => new { x.AbsenceTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AbsenceTypeLocalizations_AbsenceTypes_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalTable: "AbsenceTypes",
                        principalColumn: "AbsenceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsenceTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessAspectLocalizations",
                columns: table => new
                {
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAspectLocalizations", x => new { x.BusinessAspectId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_BusinessAspectLocalizations_BusinessAspects_BusinessAspectId",
                        column: x => x.BusinessAspectId,
                        principalTable: "BusinessAspects",
                        principalColumn: "BusinessAspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessAspectLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypeLocalizations",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    EntityTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypeLocalizations", x => new { x.EntityTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_EntityTypeLocalizations_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenderLocalizations",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    GenderDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderLocalizations", x => new { x.GenderId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_GenderLocalizations_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "GenderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenderLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseEntityRules",
                columns: table => new
                {
                    BaseEntityRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntityRules", x => x.BaseEntityRuleId);
                    table.ForeignKey(
                        name: "FK_BaseEntityRules_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypeBusinessAspects",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypeBusinessAspects", x => new { x.RuleTypeId, x.BusinessAspectId });
                    table.ForeignKey(
                        name: "FK_RuleTypeBusinessAspects_BusinessAspects_BusinessAspectId",
                        column: x => x.BusinessAspectId,
                        principalTable: "BusinessAspects",
                        principalColumn: "BusinessAspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleTypeBusinessAspects_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypeLocalizations",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    RuleTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuleTypeDescriptionDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypeLocalizations", x => new { x.RuleTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_RuleTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleTypeLocalizations_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreakTemplates",
                columns: table => new
                {
                    ShiftBreakTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftBreakTemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftBreakDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IncludedInShift = table.Column<bool>(type: "bit", nullable: false),
                    IsTimeFlexible = table.Column<bool>(type: "bit", nullable: false),
                    TemplateClicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTemplates", x => x.ShiftBreakTemplateId);
                    table.ForeignKey(
                        name: "FK_ShiftBreakTemplates_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreakTypeLocalizations",
                columns: table => new
                {
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTypeLocalizations", x => new { x.ShiftBreakTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ShiftBreakTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftBreakTypeLocalizations_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillLocalizations",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    SkillDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLocalizations", x => new { x.SkillId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_SkillLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillLocalizations_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRules",
                columns: table => new
                {
                    EntityRuleId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    RuleTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRules", x => x.EntityRuleId);
                    table.ForeignKey(
                        name: "FK_EntityRules_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRules_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShiftAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShiftStartHour = table.Column<TimeSpan>(type: "TIME", nullable: false),
                    ShiftDuration = table.Column<TimeSpan>(type: "TIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_Shifts_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkerInvitations",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InviteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SkillsetIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkerInvitations", x => new { x.EntityId, x.Email });
                    table.ForeignKey(
                        name: "FK_EntityWorkerInvitations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EntityWorkerInvitations_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkers",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    ActiveWorkerStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateSchedules = table.Column<bool>(type: "bit", nullable: false),
                    DateOfJoin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateToExit = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkers", x => new { x.EntityId, x.ApplicationUserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EntityWorkers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkers_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkers_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId");
                });

            migrationBuilder.CreateTable(
                name: "WorkerEntityAbsences",
                columns: table => new
                {
                    EntityWorkerAbsenceId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AbsenceStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOffset = table.Column<TimeSpan>(type: "time", nullable: false),
                    AbsenceApproved = table.Column<bool>(type: "bit", nullable: false),
                    AbsenceDecisionOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbsenceDateDecision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsenceDateDecisionOffset = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerEntityAbsences", x => x.EntityWorkerAbsenceId);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_AbsenceTypes_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalTable: "AbsenceTypes",
                        principalColumn: "AbsenceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseEntityRuleSpecifications",
                columns: table => new
                {
                    BaseEntityRuleId = table.Column<int>(type: "int", nullable: false),
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SpecificationValue = table.Column<int>(type: "int", nullable: false),
                    IsSpecValueBoolean = table.Column<bool>(type: "bit", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntityRuleSpecifications", x => new { x.BaseEntityRuleId, x.SpecificationId });
                    table.ForeignKey(
                        name: "FK_BaseEntityRuleSpecifications_BaseEntityRules_BaseEntityRuleId",
                        column: x => x.BaseEntityRuleId,
                        principalTable: "BaseEntityRules",
                        principalColumn: "BaseEntityRuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplateBreaks",
                columns: table => new
                {
                    ShiftTemplateId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplateBreaks", x => new { x.ShiftTemplateId, x.ShiftBreakTemplateId });
                    table.ForeignKey(
                        name: "FK_ShiftTemplateBreaks_ShiftBreakTemplates_ShiftBreakTemplateId",
                        column: x => x.ShiftBreakTemplateId,
                        principalTable: "ShiftBreakTemplates",
                        principalColumn: "ShiftBreakTemplateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftTemplateBreaks_ShiftTemplates_ShiftTemplateId",
                        column: x => x.ShiftTemplateId,
                        principalTable: "ShiftTemplates",
                        principalColumn: "ShiftTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRuleSpecifications",
                columns: table => new
                {
                    EntityRuleId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SpecificationValue = table.Column<int>(type: "int", nullable: false),
                    AspectReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false),
                    AspectReferenceId2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessAspectId2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRuleSpecifications", x => new { x.EntityRuleId, x.SpecificationId });
                    table.ForeignKey(
                        name: "FK_EntityRuleSpecifications_EntityRules_EntityRuleId",
                        column: x => x.EntityRuleId,
                        principalTable: "EntityRules",
                        principalColumn: "EntityRuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntry",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ScheduleStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntry", x => x.ScheduleEntryId);
                    table.ForeignKey(
                        name: "FK_ScheduleEntry_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreaks",
                columns: table => new
                {
                    ShiftBreakId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftBreakDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IncludedInShift = table.Column<bool>(type: "bit", nullable: false),
                    IsTimeFlexible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreaks", x => x.ShiftBreakId);
                    table.ForeignKey(
                        name: "FK_ShiftBreaks_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftBreaks_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntryWorkers",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntryWorkers", x => new { x.ScheduleEntryId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkers_ScheduleEntry_ScheduleEntryId",
                        column: x => x.ScheduleEntryId,
                        principalTable: "ScheduleEntry",
                        principalColumn: "ScheduleEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceTypeLocalizations_LocalizationId",
                table: "AbsenceTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GenderId",
                table: "AspNetUsers",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntityRules_RuleTypeId",
                table: "BaseEntityRules",
                column: "RuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessAspectLocalizations_LocalizationId",
                table: "BusinessAspectLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_EntityTypeId",
                table: "Entities",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_EntityId",
                table: "EntityRules",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_RuleTypeId",
                table: "EntityRules",
                column: "RuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypeLocalizations_LocalizationId",
                table: "EntityTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_ApplicationUserId",
                table: "EntityWorkerInvitations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_ApplicationUserId",
                table: "EntityWorkers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_SkillId",
                table: "EntityWorkers",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_GenderLocalizations_LocalizationId",
                table: "GenderLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypeBusinessAspects_BusinessAspectId",
                table: "RuleTypeBusinessAspects",
                column: "BusinessAspectId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypeLocalizations_LocalizationId",
                table: "RuleTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntry_ShiftId",
                table: "ScheduleEntry",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_ApplicationUserId",
                table: "ScheduleEntryWorkers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_ShiftBreakTypeId",
                table: "ShiftBreaks",
                column: "ShiftBreakTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_ShiftId",
                table: "ShiftBreaks",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreakTemplates_ShiftBreakTypeId",
                table: "ShiftBreakTemplates",
                column: "ShiftBreakTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreakTypeLocalizations_LocalizationId",
                table: "ShiftBreakTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EntityId",
                table: "Shifts",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTemplateBreaks_ShiftBreakTemplateId",
                table: "ShiftTemplateBreaks",
                column: "ShiftBreakTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLocalizations_LocalizationId",
                table: "SkillLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_AbsenceTypeId",
                table: "WorkerEntityAbsences",
                column: "AbsenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_ApplicationUserId",
                table: "WorkerEntityAbsences",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_EntityId",
                table: "WorkerEntityAbsences",
                column: "EntityId");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsenceTypeLocalizations");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BaseEntityRuleSpecifications");

            migrationBuilder.DropTable(
                name: "BusinessAspectLocalizations");

            migrationBuilder.DropTable(
                name: "EntityRuleSpecifications");

            migrationBuilder.DropTable(
                name: "EntityTypeLocalizations");

            migrationBuilder.DropTable(
                name: "EntityWorkerInvitations");

            migrationBuilder.DropTable(
                name: "EntityWorkers");

            migrationBuilder.DropTable(
                name: "GenderLocalizations");

            migrationBuilder.DropTable(
                name: "RuleTypeBusinessAspects");

            migrationBuilder.DropTable(
                name: "RuleTypeLocalizations");

            migrationBuilder.DropTable(
                name: "ScheduleEntryWorkers");

            migrationBuilder.DropTable(
                name: "ShiftBreaks");

            migrationBuilder.DropTable(
                name: "ShiftBreakTypeLocalizations");

            migrationBuilder.DropTable(
                name: "ShiftTemplateBreaks");

            migrationBuilder.DropTable(
                name: "SkillLocalizations");

            migrationBuilder.DropTable(
                name: "WorkerEntityAbsences");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BaseEntityRules");

            migrationBuilder.DropTable(
                name: "EntityRules");

            migrationBuilder.DropTable(
                name: "BusinessAspects");

            migrationBuilder.DropTable(
                name: "ScheduleEntry");

            migrationBuilder.DropTable(
                name: "ShiftBreakTemplates");

            migrationBuilder.DropTable(
                name: "ShiftTemplates");

            migrationBuilder.DropTable(
                name: "Localizations");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "AbsenceTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "RuleTypes");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "ShiftBreakTypes");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "EntityTypes");
        }
    }
}
