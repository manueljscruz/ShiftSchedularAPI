using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Entities.Base;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ShiftSchedularDAL.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        #region Db Sets

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<GenderLocalization> GenderLocalizations { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityWorker> EntityWorkers { get; set; }
        public DbSet<EntityWorkerSkill> EntityWorkerSkills { get; set; }
        public DbSet<EntityTypeLocalization> EntityTypeLocalizations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillLocalization> SkillLocalizations { get; set; }
        public DbSet<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public DbSet<ShiftBreakTypeLocalization> ShiftBreakTypeLocalizations { get; set; }
        public DbSet<ShiftBreakType> ShiftBreakTypes { get; set; }
        public DbSet<ShiftBreak> ShiftBreaks { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftTemplate> ShiftTemplates { get; set; }
        public DbSet<ShiftBreakTemplate> ShiftBreakTemplates { get; set; }
        public DbSet<ShiftTemplateBreaks> ShiftTemplateBreaks { get; set; }
        public DbSet<RuleType> RuleTypes { get; set; }
        public DbSet<RuleTypeLocalization> RuleTypeLocalizations { get; set; }
        public DbSet<EntityRule> EntityRules { get; set; }
        public DbSet<EntityRuleSpecification> EntityRuleSpecifications { get; set; }
        public DbSet<BusinessAspect> BusinessAspects { get; set; }
        public DbSet<BusinessAspectLocalization> BusinessAspectLocalizations { get; set; }
        public DbSet<RuleTypeBusinessAspect> RuleTypeBusinessAspects { get; set; }
        public DbSet<AbsenceType> AbsenceTypes { get; set; }
        public DbSet<AbsenceTypeLocalization> AbsenceTypeLocalizations { get; set; }
        public DbSet<EntityWorkerAbsence> WorkerEntityAbsences { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }
        public DbSet<BaseEntityRule> BaseEntityRules { get; set; }
        public DbSet<BaseEntityRuleSpecification> BaseEntityRuleSpecifications { get; set; }
        public DbSet<UserBot> UserBots { get; set; }
        public DbSet<EntityUserBot> EntityUserBots { get; set; }
        public DbSet<EntityUserBotSkill> EntityUserBotSkills { get; set; }
        public DbSet<EntityShiftRotation> EntityShiftRotations { get; set; }
        public DbSet<EntityUserBotShiftAssigned> EntityUserBotShiftAssigneds { get; set; }
        public DbSet<EntityWorkerShiftAssigned> EntityWorkerShiftAssigneds { get; set; }
        public DbSet<ScheduleEntryBots> ScheduleEntryBots { get; set; }
        public DbSet<ScheduleEntryWorkerIneligibility> ScheduleEntryWorkerIneligibilities { get; set; }
        public DbSet<ScheduleEntryBotIneligibility> scheduleEntryBotIneligibilities { get; set; }
        public DbSet<HolidayType> HolidayTypes { get; set; }
        public DbSet<HolidayTypeLocalization> HolidayTypeLocalizations { get; set; }
        public DbSet<HolidayBehaviour> HolidayBehaviours { get; set; }
        public DbSet<HolidayBehaviourLocalization> HolidayBehaviourLocalizations { get; set; }
        public DbSet<HolidayCatalog> HolidayCatalogs { get; set; }
        public DbSet<HolidayCatalogLocalization> HolidayCatalogLocalizations { get; set; }
        public DbSet<EntityHoliday> EntityHolidays { get; set; }
        public DbSet<EntityPermission> EntityPermissions { get; set; }
        public DbSet<EntityPermissionRole> EntityPermissionRoles { get; set; }
        public DbSet<EntityPermissionRoleLocalization> EntityPermissionRoleLocalizations { get; set; }

        // Notifications
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationTypeLocalization> NotificationTypeLocalizations { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        // Subscriptions & Billing
        public DbSet<SubscriptionPlanType> SubscriptionPlanTypes { get; set; }
        public DbSet<SubscriptionPlanTypeLocalization> SubscriptionPlanTypeLocalizations { get; set; }
        public DbSet<SubscriptionDurationType> SubscriptionDurationTypes { get; set; }
        public DbSet<SubscriptionDurationTypeLocalization> SubscriptionDurationTypeLocalizations { get; set; }
        public DbSet<SubscriptionPlanDurationPrice> SubscriptionPlanDurationPrices { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignLocalization> CampaignLocalizations { get; set; }
        public DbSet<CampaignSubscriptionPlan> CampaignSubscriptionPlans { get; set; }
        public DbSet<EntitySubscriptionPlan> EntitySubscriptionPlans { get; set; }
        public DbSet<SubscriptionBillingRecord> SubscriptionBillingRecords { get; set; }
        public DbSet<EntitySubscriptionPayment> EntitySubscriptionPayments { get; set; }
        public DbSet<PaymentMethodType> PaymentMethodTypes { get; set; }
        public DbSet<PaymentMethodTypeCountry> PaymentMethodTypeCountries { get; set; }
        public DbSet<PaymentMethodTypeLocalization> PaymentMethodTypeLocalizations { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ScheduleGeneration> ScheduleGenerations { get; set; }
        public DbSet<PaymentWebhookEvent> PaymentWebhookEvents { get; set; }
        public DbSet<EntityBillingProfile> EntityBillingProfiles { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        #endregion

        #region Save Changes

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditFields();
            return base.SaveChanges();
        }

        private void ApplyAuditFields()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedById = currentUserId;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedById = currentUserId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedById = currentUserId;
                        break;
                }
            }
        }

        #endregion

        #region On Model Creating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region BaseEntity Configuration

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)
                         && e.ClrType != typeof(BaseEntity)
                         && e.ClrType != typeof(BaseScheduleEntryIneligibility)))
            {
                modelBuilder.Entity(entityType.ClrType, entity =>
                {
                    entity.HasOne(typeof(ApplicationUser), "CreatedByUser")
                          .WithMany()
                          .HasForeignKey("CreatedById")
                          .IsRequired(false)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(typeof(ApplicationUser), "UpdatedByUser")
                          .WithMany()
                          .HasForeignKey("UpdatedById")
                          .IsRequired(false)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(typeof(ApplicationUser), "DeletedByUser")
                          .WithMany()
                          .HasForeignKey("DeletedById")
                          .IsRequired(false)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
                });
            }

            #endregion

            #region Application User

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Gender)
                .WithMany(g => g.ApplicationUsers)
                .HasForeignKey(a => a.GenderId);

            #endregion

            #region Gender Configuration

            modelBuilder.Entity<Gender>()
                .HasKey(g => g.GenderId);

            modelBuilder.Entity<Gender>().Property(g => g.GenderValue).HasMaxLength(100).IsRequired();

            #endregion

            #region Gender Localization Configuration

            modelBuilder.Entity<GenderLocalization>()
                .HasKey(gl => new { gl.GenderId, gl.LocalizationId });

            modelBuilder.Entity<GenderLocalization>()
                .HasOne(gl=> gl.Gender)
                .WithMany(g => g.GenderLocalizations)
                .HasForeignKey(gl => gl.GenderId);

            modelBuilder.Entity<GenderLocalization>()
                .HasOne(gl => gl.Localization)
                .WithMany(l => l.GenderLocalizations)
                .HasForeignKey(gl => gl.LocalizationId);

            #endregion

            #region Entity Configuration

            modelBuilder.Entity<Entity>()
                .HasKey(e => e.EntityId);

            modelBuilder.Entity<Entity>()
                .HasOne(e => e.ParentEntity)
                .WithMany(pe => pe.ChildrenEntities)
                .HasForeignKey(e => e.ParentEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Skill Localization Configuration

            modelBuilder.Entity<SkillLocalization>()
                .HasKey(sl => new { sl.SkillId, sl.LocalizationId });

            modelBuilder.Entity<SkillLocalization>()
                .HasOne(sl => sl.Skill)
                .WithMany(s => s.SkillLocalizations)
                .HasForeignKey(sl => sl.SkillId);

            modelBuilder.Entity<SkillLocalization>()
                .HasOne(sl => sl.Localization)
                .WithMany(l => l.SkillLocalizations)
                .HasForeignKey(sl => sl.LocalizationId);

            #endregion

            #region Localization Configuration

            modelBuilder.Entity<Localization>()
                .HasKey(l => l.LocalizationId);

            #endregion

            #region Entity Type

            modelBuilder.Entity<EntityType>()
                .HasKey(et => et.EntityTypeId);

            #endregion

            #region Entity Type Localization

            modelBuilder.Entity<EntityTypeLocalization>()
                .HasKey(etl => new { etl.EntityTypeId, etl.LocalizationId });

            modelBuilder.Entity<EntityTypeLocalization>()
                .HasOne(etl => etl.EntityType)
                .WithMany(et => et.EntityTypeLocalizations)
                .HasForeignKey(etl => etl.EntityTypeId);

            modelBuilder.Entity<EntityTypeLocalization>()
                .HasOne(etl => etl.Localization)
                .WithMany(l => l.EntityTypeLocalizations)
                .HasForeignKey(etl => etl.LocalizationId);

            #endregion

            #region Skill Configuration

            modelBuilder.Entity<Skill>()
                .HasKey(s => s.SkillId);

            #endregion

            #region Entity Workers

            modelBuilder.Entity<EntityWorker>()
                .HasKey(ew => new { ew.EntityId, ew.ApplicationUserId });

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.Entity)
                .WithMany(e => e.EntityWorkers)
                .HasForeignKey(ew => ew.EntityId);

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.ApplicationUser)
                .WithMany(e => e.EntityWorkers)
                .HasForeignKey(ew => ew.ApplicationUserId);

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.ConvertedByUser)
                .WithMany()
                .HasForeignKey(ew => ew.ConvertedBy)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.ConvertedFromBot)
                .WithMany()
                .HasForeignKey(ew => ew.ConvertedFromBotId)
                .HasPrincipalKey(ub => ub.UserBotId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Entity Workers Skills

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasKey(ews => new { ews.ApplicationUserId, ews.EntityId, ews.SkillId });

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasOne(ews => ews.Entity)
                .WithMany(e => e.EntityWorkerSkills)
                .HasForeignKey(ews => ews.EntityId);

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasOne(ews => ews.ApplicationUser)
                .WithMany(w => w.EntityWorkerSkills)
                .HasForeignKey(ews => ews.ApplicationUserId);

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasOne(ews => ews.Skill)
                .WithMany(s => s.EntityWorkerSkills)
                .HasForeignKey(ews => ews.SkillId);

            #endregion

            #region Entity User Bots

            modelBuilder.Entity<EntityUserBot>()
                .HasKey(eub => new { eub.EntityId, eub.UserBotId });

            modelBuilder.Entity<EntityUserBot>()
                .HasOne(eub => eub.Entity)
                .WithMany(e => e.EntityUserBots)
                .HasForeignKey(eub => eub.EntityId);

            modelBuilder.Entity<EntityUserBot>()
                .HasOne(eub => eub.UserBot)
                .WithMany(ub => ub.EntityUserBots)
                .HasForeignKey(eub => eub.UserBotId);

            #endregion

            #region Entity User Bot Skills

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasKey(eubs => new { eubs.EntityId, eubs.UserBotId, eubs.SkillId });

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasOne(eubs => eubs.Entity)
                .WithMany(e => e.EntityUserBotSkills)
                .HasForeignKey(eubs => eubs.EntityId);

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasOne(eubs => eubs.UserBot)
                .WithMany(ub => ub.EntityUserBotSkills)
                .HasForeignKey(eubs => eubs.UserBotId);

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasOne(eubs => eubs.Skill)
                .WithMany(s => s.EntityUserBotSkills)
                .HasForeignKey(eubs => eubs.SkillId);

            #endregion

            #region Entity Worker Invitation Configuration

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasKey(ewi => new { ewi.EntityId, ewi.Email });

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasOne(ewi => ewi.Entity)
                .WithMany(e => e.EntityWorkerInvitations)
                .HasForeignKey(ewi => ewi.EntityId);

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasOne(ewi => ewi.ApplicationUser)
                .WithMany(w => w.EntityWorkerInvitations)
                .HasForeignKey(ewi => ewi.ApplicationUserId);

            #endregion

            #region Shift Break Type Configuration

            modelBuilder.Entity<ShiftBreakType>()
                .HasKey(sbt => sbt.ShiftBreakTypeId);

            #endregion

            #region Shift Break Type Localization Configuration

            modelBuilder.Entity<ShiftBreakTypeLocalization>()
                .HasKey(sbtl => new { sbtl.ShiftBreakTypeId, sbtl.LocalizationId });

            modelBuilder.Entity<ShiftBreakTypeLocalization>()
                .HasOne(sbtl => sbtl.ShiftBreakType)
                .WithMany(sbt => sbt.ShiftBreakTypeLocalizations)
                .HasForeignKey(sbtl => sbtl.ShiftBreakTypeId);

            modelBuilder.Entity<ShiftBreakTypeLocalization>()
                .HasOne(sbtl => sbtl.Localization)
                .WithMany(l => l.ShiftBreakTypeLocalizations)
                .HasForeignKey(sbtl => sbtl.LocalizationId);

            #endregion

            #region Shift Configuration

            modelBuilder.Entity<Shift>()
                .HasKey(s => s.ShiftId);

            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Entity)
                .WithMany(e => e.EntityShifts)
                .HasForeignKey(s => s.EntityId);

            #endregion

            #region Shift Breaks Configuration

            modelBuilder.Entity<ShiftBreak>()
                .HasKey(sb => sb.ShiftBreakId);

            modelBuilder.Entity<ShiftBreak>()
                .HasOne(sb => sb.Shift)
                .WithMany(s => s.ShiftBreaks)
                .HasForeignKey(sb => sb.ShiftId);

            modelBuilder.Entity<ShiftBreak>()
                .HasOne(sb => sb.ShiftBreakType)
                .WithMany(sbt => sbt.ShiftBreaks)
                .HasForeignKey(sb => sb.ShiftBreakTypeId);

            #endregion

            #region Shift Template Configuration

            modelBuilder.Entity<ShiftTemplate>()
                .HasKey(st => st.ShiftTemplateId);

            #endregion

            #region Shift Break Template Configuration


            modelBuilder.Entity<ShiftBreakTemplate>()
                .HasKey(sbt => sbt.ShiftBreakTemplateId);

            modelBuilder.Entity<ShiftBreakTemplate>()
                .HasOne(sbt => sbt.ShiftBreakType)
                .WithMany(st => st.ShiftBreakTemplates)
                .HasForeignKey(sbt => sbt.ShiftBreakTypeId);

            #endregion

            #region Shift Template Breaks Configuration

            modelBuilder.Entity<ShiftTemplateBreaks>()
                .HasKey(stb => new { stb.ShiftTemplateId, stb.ShiftBreakTemplateId });

            modelBuilder.Entity<ShiftTemplateBreaks>()
                .HasOne(stb => stb.ShiftTemplate)
                .WithMany(st => st.ShiftTemplateBreaks)
                .HasForeignKey(stb => stb.ShiftTemplateId);

            modelBuilder.Entity<ShiftTemplateBreaks>()
                .HasOne(stb => stb.ShiftBreakTemplate)
                .WithMany(sb => sb.ShiftTemplateBreaks)
                .HasForeignKey(stb => stb.ShiftBreakTemplateId);

            #endregion

            #region Business Aspect Configuration

            modelBuilder.Entity<BusinessAspect>()
                .HasKey(ba => ba.BusinessAspectId);

            #endregion

            #region Business Aspect Localization Configuration

            modelBuilder.Entity<BusinessAspectLocalization>()
                .HasKey(bal => new { bal.BusinessAspectId, bal.LocalizationId });

            modelBuilder.Entity<BusinessAspectLocalization>()
                .HasOne(bal => bal.BusinessAspect)
                .WithMany(ba => ba.BusinessAspectLocalizations)
                .HasForeignKey(bal => bal.BusinessAspectId);

            modelBuilder.Entity<BusinessAspectLocalization>()
                .HasOne(bal => bal.Localization)
                .WithMany(l => l.BusinessAspectLocalizations)
                .HasForeignKey(bal => bal.LocalizationId);

            #endregion

            #region Rule Type Configuration

            modelBuilder.Entity<RuleType>()
                .HasKey(rt => rt.RuleTypeId);

            #endregion

            #region Rule Type Localization Configuration

            modelBuilder.Entity<RuleTypeLocalization>()
                .HasKey(rtl => new { rtl.RuleTypeId, rtl.LocalizationId });

            modelBuilder.Entity<RuleTypeLocalization>()
                .HasOne(rtl => rtl.RuleType)
                .WithMany(rt => rt.RuleTypeLocalizations)
                .HasForeignKey(rtl => rtl.RuleTypeId);

            modelBuilder.Entity<RuleTypeLocalization>()
                .HasOne(rtl => rtl.Localization)
                .WithMany(l => l.RuleTypeLocalizations)
                .HasForeignKey(rtl => rtl.LocalizationId);

            #endregion

            #region Entity Rule Configuration

            modelBuilder.Entity<EntityRule>()
                .HasKey(er => er.EntityRuleId);

            modelBuilder.Entity<EntityRule>()
                .HasOne(er => er.Entity)
                .WithMany(e => e.EntityRules)
                .HasForeignKey(er => er.EntityId);

            modelBuilder.Entity<EntityRule>()
                .HasOne(er => er.RuleType)
                .WithMany(rt => rt.EntityRules)
                .HasForeignKey(er => er.RuleTypeId);

            #endregion

            #region Entity Rule Specifications

            modelBuilder.Entity<EntityRuleSpecification>()
                .HasKey(ers => new { ers.EntityRuleId, ers.SpecificationId });

            modelBuilder.Entity<EntityRuleSpecification>()
                .HasOne(ers => ers.EntityRule)
                .WithMany(er => er.EntityRuleSpecifications)
                .HasForeignKey(ers => ers.EntityRuleId);

            #endregion

            #region Rule Type Business Aspect Configuration

            modelBuilder.Entity<RuleTypeBusinessAspect>()
                .HasKey(rtba => new { rtba.RuleTypeId, rtba.BusinessAspectId });

            modelBuilder.Entity<RuleTypeBusinessAspect>()
                .HasOne(rtba => rtba.RuleType)
                .WithMany(rt => rt.RuleTypeBusinessAspects)
                .HasForeignKey(rtba => rtba.RuleTypeId);

            modelBuilder.Entity<RuleTypeBusinessAspect>()
                .HasOne(rtba => rtba.BusinessAspect)
                .WithMany(ba => ba.RuleTypeBusinessAspects)
                .HasForeignKey(rtba => rtba.BusinessAspectId);

            #endregion

            #region Absence Type Configuration

            modelBuilder.Entity<AbsenceType>()
                .HasKey(at => at.AbsenceTypeId);

            #endregion

            #region Absence Type Localization Configuration

            modelBuilder.Entity<AbsenceTypeLocalization>()
                .HasKey(atl => new { atl.AbsenceTypeId, atl.LocalizationId });

            modelBuilder.Entity<AbsenceTypeLocalization>()
                .HasOne(atl => atl.AbsenceType)
                .WithMany(at => at.AbsenceTypeLocalizations)
                .HasForeignKey(atl => atl.AbsenceTypeId);

            modelBuilder.Entity<AbsenceTypeLocalization>()
                .HasOne(atl => atl.Localization)
                .WithMany(l => l.AbsenceTypeLocalizations)
                .HasForeignKey(atl => atl.LocalizationId);

            #endregion

            #region Entity Worker Absences Configuration

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasKey(ewa => ewa.EntityWorkerAbsenceId);

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasOne(ewa => ewa.ApplicationUser)
                .WithMany(w => w.EntityWorkerAbsences)
                .HasForeignKey(ewa => ewa.ApplicationUserId);

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasOne(ewa => ewa.Entity)
                .WithMany(e => e.EntityWorkerAbsences)
                .HasForeignKey(ewa => ewa.EntityId);

            #endregion

            #region Schedule Entry Configuration

            modelBuilder.Entity<ScheduleEntry>()
                .ToTable("ScheduleEntry")
                .HasKey(se => se.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(se => se.Shift)
                .WithMany(s => s.ScheduleEntries)
                .HasForeignKey(se => se.ShiftId);

            #endregion

            #region Schedule Entry Workers Configuration

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasKey(sew => new { sew.ScheduleEntryId, sew.ApplicationUserId });

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasOne(sew => sew.ScheduleEntry)
                .WithMany(se => se.ScheduleEntryWorkers)
                .HasForeignKey(sew => sew.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasOne(sew => sew.ApplicationUser)
                .WithMany(w => w.ScheduleEntryWorkers)
                .HasForeignKey(sew => sew.ApplicationUserId);

            #endregion

            #region Base Entity Rule Configuration

            modelBuilder.Entity<BaseEntityRule>()
                .HasKey(ber => ber.BaseEntityRuleId);

            modelBuilder.Entity<BaseEntityRule>()
                .HasOne(ber => ber.RuleType)
                .WithMany(e => e.BaseEntityRules)
                .HasForeignKey(ber => ber.RuleTypeId);

            #endregion

            #region Base Entity Rule Specification Configuration

            modelBuilder.Entity<BaseEntityRuleSpecification>()
                .HasKey(bers => new { bers.BaseEntityRuleId, bers.SpecificationId });

            modelBuilder.Entity<BaseEntityRuleSpecification>()
                .HasOne(bers => bers.BaseEntityRule)
                .WithMany(ber => ber.BaseEntityRuleSpecifications)
                .HasForeignKey(bers => bers.BaseEntityRuleId);

            #endregion

            #region User Bot Configuration

            modelBuilder.Entity<UserBot>()
                .HasKey(ub => ub.UserBotId);

            #endregion

            #region Entity Shift Rotation Configuration

            modelBuilder.Entity<EntityShiftRotation>()
                .HasKey(esr => new { esr.EntityId, esr.OrderNo, esr.IsLeave });

            modelBuilder.Entity<EntityShiftRotation>()
                .HasOne(esr => esr.Entity)
                .WithMany(e => e.EntityShiftRotations)
                .HasForeignKey(esr => esr.EntityId);

            modelBuilder.Entity<EntityShiftRotation>()
                .HasOne(esr => esr.Shift)
                .WithMany()
                .HasForeignKey(esr => esr.ShiftId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            #endregion

            #region Entity User Bot Shift Assigned Configuration

            modelBuilder.Entity<EntityUserBotShiftAssigned>()
                .HasKey(eubsa => new { eubsa.UserBotId, eubsa.EntityId, eubsa.ShiftId });

            modelBuilder.Entity<EntityUserBotShiftAssigned>()
                .HasOne(eubsa => eubsa.UserBot)
                .WithMany(ub => ub.EntityUserBotShiftAssigneds)
                .HasForeignKey(eubsa => eubsa.UserBotId);

            modelBuilder.Entity<EntityUserBotShiftAssigned>()
                .HasOne(eubsa => eubsa.Shift)
                .WithMany(ub => ub.EntityUserBotShiftAssigned)
                .HasForeignKey(eubsa => eubsa.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntityUserBotShiftAssigned>()
                .HasOne(eubsa => eubsa.Entity)
                .WithMany(ub => ub.EntityUserBotShiftAssigneds)
                .HasForeignKey(eubsa => eubsa.EntityId);

            #endregion

            #region Entity Worker Shift Assigned Configuration


            modelBuilder.Entity<EntityWorkerShiftAssigned>()
                .HasKey(ewsa => new { ewsa.ApplicationUserId, ewsa.EntityId, ewsa.ShiftId });

            modelBuilder.Entity<EntityWorkerShiftAssigned>()
                .HasOne(ewsa => ewsa.ApplicationUser)
                .WithMany(w => w.EntityWorkerShiftAssigneds)
                .HasForeignKey(ewsa => ewsa.ApplicationUserId);

            modelBuilder.Entity<EntityWorkerShiftAssigned>()
                .HasOne(ewsa => ewsa.Shift)
                .WithMany(s => s.EntityWorkerShiftAssigned)
                .HasForeignKey(ewsa => ewsa.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntityWorkerShiftAssigned>()
                .HasOne(ewsa => ewsa.Entity)
                .WithMany(e => e.EntityWorkerShiftAssigneds)
                .HasForeignKey(ewsa => ewsa.EntityId);

            #endregion

            #region Schedule Entry Bots Configuration

            modelBuilder.Entity<ScheduleEntryBots>()
                .HasKey(seb => new { seb.ScheduleEntryId, seb.UserBotId });

            modelBuilder.Entity<ScheduleEntryBots>()
                .HasOne(seb => seb.ScheduleEntry)
                .WithMany(se => se.ScheduleEntryBots)
                .HasForeignKey(seb => seb.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntryBots>()
                .HasOne(seb => seb.UserBot)
                .WithMany(ub => ub.ScheduleEntryBots)
                .HasForeignKey(seb => seb.UserBotId);

            #endregion

            #region Schedule Entry Bot Ineligibility

            modelBuilder.Entity<ScheduleEntryBotIneligibility>()
                .HasKey(sebi => new { sebi.ScheduleEntryId, sebi.UserBotId });

            modelBuilder.Entity<ScheduleEntryBotIneligibility>()
                .HasOne(se => se.ScheduleEntry)
                .WithMany(sebi => sebi.ScheduleEntryBotIneligibilities)
                .HasForeignKey(se => se.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntryBotIneligibility>()
                .HasOne(sebi => sebi.UserBot)
                .WithMany(ub => ub.ScheduleEntryBotIneligibilities)
                .HasForeignKey(sebi => sebi.UserBotId);

            #endregion

            #region Schedule Entry Worker Ineligibility

            modelBuilder.Entity<ScheduleEntryWorkerIneligibility>()
                .HasKey(sewi => new { sewi.ScheduleEntryId, sewi.ApplicationUserId });

            modelBuilder.Entity<ScheduleEntryWorkerIneligibility>()
                .HasOne(se => se.ScheduleEntry)
                .WithMany(sewi => sewi.ScheduleEntryWorkerIneligibilities)
                .HasForeignKey(se => se.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntryWorkerIneligibility>()
                .HasOne(sewi => sewi.ApplicationUser)
                .WithMany(au => au.ScheduleEntryWorkerIneligibilities)
                .HasForeignKey(sewi => sewi.ApplicationUserId);

            #endregion

            #region Holiday Type Configuration

            modelBuilder.Entity<HolidayType>()
                .HasKey(ht => ht.HolidayTypeId);

            #endregion

            #region Holiday Type Localization Configuration

            modelBuilder.Entity<HolidayTypeLocalization>()
                .HasKey(htl => new { htl.HolidayTypeId, htl.LocalizationId });

            modelBuilder.Entity<HolidayTypeLocalization>()
                .HasOne(htl => htl.HolidayType)
                .WithMany(ht => ht.HolidayTypeLocalizations)
                .HasForeignKey(htl => htl.HolidayTypeId);

            modelBuilder.Entity<HolidayTypeLocalization>()
                .HasOne(htl => htl.Localization)
                .WithMany(l => l.HolidayTypeLocalizations)
                .HasForeignKey(htl => htl.LocalizationId);

            #endregion

            #region Holiday Behaviour Configuration

            modelBuilder.Entity<HolidayBehaviour>()
                .HasKey(hb => hb.HolidayBehaviourId);

            #endregion

            #region Holiday Behaviour Localization Configuration

            modelBuilder.Entity<HolidayBehaviourLocalization>()
                .HasKey(hbl => new { hbl.HolidayBehaviourId, hbl.LocalizationId });

            modelBuilder.Entity<HolidayBehaviourLocalization>()
                .HasOne(hbl => hbl.HolidayBehaviour)
                .WithMany(hb => hb.HolidayBehaviourLocalizations)
                .HasForeignKey(hbl => hbl.HolidayBehaviourId);

            modelBuilder.Entity<HolidayBehaviourLocalization>()
                .HasOne(hbl => hbl.Localization)
                .WithMany(l => l.HolidayBehaviourLocalizations)
                .HasForeignKey(hbl => hbl.LocalizationId);

            #endregion

            #region Holiday Catalog Configuration

            modelBuilder.Entity<HolidayCatalog>()
                .HasKey(hc => hc.HolidayCatalogId);

            modelBuilder.Entity<HolidayCatalog>()
                .HasOne(hc => hc.HolidayType)
                .WithMany(ht => ht.HolidayCatalogs)
                .HasForeignKey(hc => hc.HolidayTypeId);

            modelBuilder.Entity<HolidayCatalog>()
                .HasOne(hc => hc.HolidayBehaviour)
                .WithMany(hb => hb.HolidayCatalogs)
                .HasForeignKey(hc => hc.HolidayBehaviourId);

            #endregion

            #region Holiday Catalog Localization Configuration

            modelBuilder.Entity<HolidayCatalogLocalization>()
                .HasKey(hcl => new { hcl.HolidayCatalogId, hcl.LocalizationId });

            modelBuilder.Entity<HolidayCatalogLocalization>()
                .HasOne(hcl => hcl.HolidayCatalog)
                .WithMany(hc => hc.HolidayCatalogLocalizations)
                .HasForeignKey(hcl => hcl.HolidayCatalogId);

            modelBuilder.Entity<HolidayCatalogLocalization>()
                .HasOne(hcl => hcl.Localization)
                .WithMany(l => l.HolidayCatalogLocalizations)
                .HasForeignKey(hcl => hcl.LocalizationId);

            #endregion

            #region Entity Holiday Configuration

            modelBuilder.Entity<EntityHoliday>()
                .HasKey(eh => eh.EntityHolidayId);

            modelBuilder.Entity<EntityHoliday>()
                .HasOne(eh => eh.Entity)
                .WithMany(e => e.EntityHolidays)
                .HasForeignKey(eh => eh.EntityId);

            modelBuilder.Entity<EntityHoliday>()
                .HasOne(eh => eh.HolidayCatalog)
                .WithMany(hc => hc.EntityHolidays)
                .HasForeignKey(eh => eh.HolidayCatalogId)
                .IsRequired(false);

            modelBuilder.Entity<EntityHoliday>()
                .HasOne(eh => eh.HolidayBehaviour)
                .WithMany(hb => hb.EntityHolidays)
                .HasForeignKey(eh => eh.HolidayBehaviourId);

            #endregion

            #region Entity Permission Role Configuration

            modelBuilder.Entity<EntityPermissionRole>()
                .HasKey(epr => epr.EntityPermissionRoleId);

            #endregion

            #region Entity Permission Role Localization Configuration

            modelBuilder.Entity<EntityPermissionRoleLocalization>()
                .HasKey(epr => new { epr.EntityPermissionRoleId, epr.LocalizationId });

            modelBuilder.Entity<EntityPermissionRoleLocalization>()
                .HasOne(eprl => eprl.Localization)
                .WithMany(l => l.EntityPermissionRoleLocalizations)
                .HasForeignKey(eprl => eprl.LocalizationId);

            modelBuilder.Entity<EntityPermissionRoleLocalization>()
                .HasOne(eprl => eprl.EntityPermissionRole)
                .WithMany(epr => epr.EntityPermissionRoleLocalizations)
                .HasForeignKey(eprl => eprl.EntityPermissionRoleId);

            #endregion

            #region Entity Permission Configuration

            modelBuilder.Entity<EntityPermission>()
                .HasKey(ep => new { ep.EntityId, ep.ApplicationUserId, ep.EntityPermissionRoleId });

            modelBuilder.Entity<EntityPermission>()
                .HasOne(ep => ep.Entity)
                .WithMany(e => e.EntityPermissions)
                .HasForeignKey(ep => ep.EntityId);

            modelBuilder.Entity<EntityPermission>()
                .HasOne(ep => ep.EntityPermissionRole)
                .WithMany(epr => epr.EntityPermissions)
                .HasForeignKey(ep => ep.EntityPermissionRoleId);

            modelBuilder.Entity<EntityPermission>()
                .HasOne(ep => ep.ApplicationUser)
                .WithMany(au => au.EntityPermissions)
                .HasForeignKey(ep => ep.ApplicationUserId);

            #endregion

            #region Database Indexes for Performance

            // HolidayCatalog indexes
            modelBuilder.Entity<HolidayCatalog>()
                .HasIndex(hc => hc.HolidayTypeId)
                .HasDatabaseName("IX_HolidayCatalogs_HolidayTypeId");

            modelBuilder.Entity<HolidayCatalog>()
                .HasIndex(hc => hc.HolidayBehaviourId)
                .HasDatabaseName("IX_HolidayCatalogs_HolidayBehaviourId");

            modelBuilder.Entity<HolidayCatalog>()
                .HasIndex(hc => new { hc.RecurrenceMonth, hc.RecurrenceDay })
                .HasDatabaseName("IX_HolidayCatalogs_RecurrenceDate");

            // EntityHoliday indexes - frequently queried by entity
            modelBuilder.Entity<EntityHoliday>()
                .HasIndex(eh => eh.EntityId)
                .HasDatabaseName("IX_EntityHolidays_EntityId");

            modelBuilder.Entity<EntityHoliday>()
                .HasIndex(eh => eh.HolidayCatalogId)
                .HasDatabaseName("IX_EntityHolidays_HolidayCatalogId");

            modelBuilder.Entity<EntityHoliday>()
                .HasIndex(eh => eh.HolidayBehaviourId)
                .HasDatabaseName("IX_EntityHolidays_HolidayBehaviourId");

            modelBuilder.Entity<EntityHoliday>()
                .HasIndex(eh => new { eh.EntityId, eh.CustomMonth, eh.CustomDay })
                .HasDatabaseName("IX_EntityHolidays_EntityId_CustomDate");

            // EntityWorker indexes - highly queried table
            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => ew.EntityId)
                .HasDatabaseName("IX_EntityWorkers_EntityId");

            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => ew.ApplicationUserId)
                .HasDatabaseName("IX_EntityWorkers_ApplicationUserId");

            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => new { ew.EntityId, ew.ApplicationUserId })
                .HasDatabaseName("IX_EntityWorkers_EntityId_ApplicationUserId");

            // EntityWorkerSkill indexes
            modelBuilder.Entity<EntityWorkerSkill>()
                .HasIndex(ews => ews.EntityId)
                .HasDatabaseName("IX_EntityWorkerSkills_EntityId");

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasIndex(ews => ews.ApplicationUserId)
                .HasDatabaseName("IX_EntityWorkerSkills_ApplicationUserId");

            modelBuilder.Entity<EntityWorkerSkill>()
                .HasIndex(ews => ews.SkillId)
                .HasDatabaseName("IX_EntityWorkerSkills_SkillId");

            // Shift indexes
            modelBuilder.Entity<Shift>()
                .HasIndex(s => s.EntityId)
                .HasDatabaseName("IX_Shifts_EntityId");

            // ScheduleEntry indexes - critical for date range queries
            modelBuilder.Entity<ScheduleEntry>()
                .HasIndex(se => se.ShiftId)
                .HasDatabaseName("IX_ScheduleEntries_ShiftId");

            modelBuilder.Entity<ScheduleEntry>()
                .HasIndex(se => new { se.ScheduleStartDate, se.ScheduleEndDate })
                .HasDatabaseName("IX_ScheduleEntries_DateRange");

            modelBuilder.Entity<ScheduleEntry>()
                .HasIndex(se => new { se.ShiftId, se.ScheduleStartDate, se.ScheduleEndDate })
                .HasDatabaseName("IX_ScheduleEntries_ShiftId_DateRange");

            // EntityWorkerAbsence indexes
            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasIndex(ewa => ewa.EntityId)
                .HasDatabaseName("IX_EntityWorkerAbsences_EntityId");

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasIndex(ewa => ewa.ApplicationUserId)
                .HasDatabaseName("IX_EntityWorkerAbsences_ApplicationUserId");

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasIndex(ewa => new { ewa.AbsenceStartDate, ewa.AbsenceEndDate })
                .HasDatabaseName("IX_EntityWorkerAbsences_DateRange");

            // EntityWorkerInvitation indexes
            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasIndex(ewi => ewi.Email)
                .HasDatabaseName("IX_EntityWorkerInvitations_Email");

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasIndex(ewi => ewi.EntityId)
                .HasDatabaseName("IX_EntityWorkerInvitations_EntityId");

            // ScheduleEntryWorkers indexes
            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasIndex(sew => sew.ScheduleEntryId)
                .HasDatabaseName("IX_ScheduleEntryWorkers_ScheduleEntryId");

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasIndex(sew => sew.ApplicationUserId)
                .HasDatabaseName("IX_ScheduleEntryWorkers_ApplicationUserId");

            // EntityUserBot indexes (bot equivalent of worker indexes)
            modelBuilder.Entity<EntityUserBot>()
                .HasIndex(eub => eub.EntityId)
                .HasDatabaseName("IX_EntityUserBots_EntityId");

            modelBuilder.Entity<EntityUserBot>()
                .HasIndex(eub => eub.UserBotId)
                .HasDatabaseName("IX_EntityUserBots_UserBotId");

            // EntityUserBotSkill indexes
            modelBuilder.Entity<EntityUserBotSkill>()
                .HasIndex(eubs => eubs.EntityId)
                .HasDatabaseName("IX_EntityUserBotSkills_EntityId");

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasIndex(eubs => eubs.UserBotId)
                .HasDatabaseName("IX_EntityUserBotSkills_UserBotId");

            modelBuilder.Entity<EntityUserBotSkill>()
                .HasIndex(eubs => eubs.SkillId)
                .HasDatabaseName("IX_EntityUserBotSkills_SkillId");

            // EntityShiftRotation indexes
            modelBuilder.Entity<EntityShiftRotation>()
                .HasIndex(esr => esr.EntityId)
                .HasDatabaseName("IX_EntityShiftRotations_EntityId");

            modelBuilder.Entity<EntityShiftRotation>()
                .HasIndex(esr => esr.ShiftId)
                .HasDatabaseName("IX_EntityShiftRotations_ShiftId");

            // EntityRule indexes
            modelBuilder.Entity<EntityRule>()
                .HasIndex(er => er.EntityId)
                .HasDatabaseName("IX_EntityRules_EntityId");

            modelBuilder.Entity<EntityRule>()
                .HasIndex(er => er.RuleTypeId)
                .HasDatabaseName("IX_EntityRules_RuleTypeId");

            // EntityWorker conversion indexes
            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => ew.ConvertedFromBotId)
                .HasDatabaseName("IX_EntityWorkers_ConvertedFromBotId");

            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => ew.ConvertedBy)
                .HasDatabaseName("IX_EntityWorkers_ConvertedBy");

            // Entity Permission Indexes
            modelBuilder.Entity<EntityPermission>()
                .HasIndex(ep => ep.ApplicationUserId)
                .HasDatabaseName("IX_EntityPermissions_ApplicationUserId");

            modelBuilder.Entity<EntityPermission>()
                .HasIndex(ep => new { ep.EntityId, ep.EntityPermissionRoleId })
                .HasDatabaseName("IX_EntityPermissions_EntityId_RoleId");

            // Entity hierarchy index - supports loading children of a parent entity
            modelBuilder.Entity<Entity>()
                .HasIndex(e => e.ParentEntityId)
                .HasDatabaseName("IX_Entities_ParentEntityId");

            // Soft delete filtered indexes - only index active (non-deleted) rows
            modelBuilder.Entity<EntityWorker>()
                .HasIndex(ew => ew.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_EntityWorkers_IsDeleted");

            modelBuilder.Entity<UserBot>()
                .HasIndex(ub => ub.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_UserBots_IsDeleted");

            modelBuilder.Entity<EntityUserBot>()
                .HasIndex(eub => eub.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_EntityUserBots_IsDeleted");

            modelBuilder.Entity<ScheduleEntry>()
                .HasIndex(se => se.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_ScheduleEntries_IsDeleted");

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasIndex(sew => sew.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_ScheduleEntryWorkers_IsDeleted");

            modelBuilder.Entity<ScheduleEntryBots>()
                .HasIndex(seb => seb.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_ScheduleEntryBots_IsDeleted");

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasIndex(ewa => ewa.IsDeleted)
                .HasFilter("IsDeleted = 0")
                .HasDatabaseName("IX_EntityWorkerAbsences_IsDeleted");

            #endregion

            #region Notification Configuration

            // NotificationType
            modelBuilder.Entity<NotificationType>()
                .HasKey(nt => nt.NotificationTypeId);

            modelBuilder.Entity<NotificationType>()
                .Property(nt => nt.NotificationTypeCode)
                .HasMaxLength(100)
                .IsRequired();

            // NotificationTypeLocalization
            modelBuilder.Entity<NotificationTypeLocalization>()
                .HasKey(ntl => new { ntl.LocalizationId, ntl.NotificationTypeId});

            modelBuilder.Entity<NotificationTypeLocalization>()
                .HasOne(ntl => ntl.NotificationType)
                .WithMany(nt => nt.NotificationTypeLocalizations)
                .HasForeignKey(ntl => ntl.NotificationTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationTypeLocalization>()
                .HasOne(ntl => ntl.Localization)
                .WithMany(l => l.NotificationTypeLocalizations)
                .HasForeignKey(ntl => ntl.LocalizationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // UserNotification
            modelBuilder.Entity<UserNotification>()
                .HasKey(un => un.UserNotificationId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.NotificationType)
                .WithMany(nt => nt.UserNotifications)
                .HasForeignKey(un => un.NotificationTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserNotification>()
                .HasIndex(un => un.UserId)
                .HasDatabaseName("IX_UserNotifications_UserId");

            modelBuilder.Entity<UserNotification>()
                .HasIndex(un => new { un.UserId, un.IsRead })
                .HasDatabaseName("IX_UserNotifications_UserId_IsRead");

            // Seed data — NotificationTypes and their localizations
            modelBuilder.Entity<NotificationType>().HasData(
                new NotificationType { NotificationTypeId = 1, NotificationTypeCode = NotificationTypeCodes.InvitationReceived },
                new NotificationType { NotificationTypeId = 2, NotificationTypeCode = NotificationTypeCodes.AbsenceApproved },
                new NotificationType { NotificationTypeId = 3, NotificationTypeCode = NotificationTypeCodes.AbsenceDeclined },
                new NotificationType { NotificationTypeId = 4, NotificationTypeCode = NotificationTypeCodes.ScheduleAssigned },
                new NotificationType { NotificationTypeId = 5, NotificationTypeCode = NotificationTypeCodes.ExitDateSet }
            );

            // Seed localizations keyed by LocalizationId (1=en, 2=fr, 3=es, 4=de, 5=it, 6=pt)
            // Inner tuple: (NotificationTypeDisplayValue, MessageTemplate)
            var notificationLocalizations = new Dictionary<int, Dictionary<int, (string DisplayValue, string MessageTemplate)>>
            {
                [1] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Invitation Received",       "You have been invited to join {0}."),
                    [2] = ("Invitation reçue",           "Vous avez été invité à rejoindre {0}."),
                    [3] = ("Invitación recibida",        "Has sido invitado a unirte a {0}."),
                    [4] = ("Einladung erhalten",         "Sie wurden eingeladen, {0} beizutreten."),
                    [5] = ("Invito ricevuto",            "Sei stato invitato a unirti a {0}."),
                    [6] = ("Convite recebido",           "Você foi convidado para se juntar a {0}.")
                },
                [2] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Absence Approved",          "Your absence request has been approved."),
                    [2] = ("Absence approuvée",          "Votre demande d'absence a été approuvée."),
                    [3] = ("Ausencia aprobada",          "Tu solicitud de ausencia ha sido aprobada."),
                    [4] = ("Abwesenheit genehmigt",      "Ihr Abwesenheitsantrag wurde genehmigt."),
                    [5] = ("Assenza approvata",          "La tua richiesta di assenza è stata approvata."),
                    [6] = ("Ausência aprovada",          "A sua solicitação de ausência foi aprovada.")
                },
                [3] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Absence Declined",          "Your absence request has been declined."),
                    [2] = ("Absence refusée",            "Votre demande d'absence a été refusée."),
                    [3] = ("Ausencia rechazada",         "Tu solicitud de ausencia ha sido rechazada."),
                    [4] = ("Abwesenheit abgelehnt",      "Ihr Abwesenheitsantrag wurde abgelehnt."),
                    [5] = ("Assenza rifiutata",          "La tua richiesta di assenza è stata rifiutata."),
                    [6] = ("Ausência recusada",          "A sua solicitação de ausência foi recusada.")
                },
                [4] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Shift Assigned",            "You have been assigned to a shift at {0}."),
                    [2] = ("Quart attribué",             "Vous avez été affecté à un quart de travail chez {0}."),
                    [3] = ("Turno asignado",             "Has sido asignado a un turno en {0}."),
                    [4] = ("Schicht zugewiesen",         "Sie wurden einer Schicht bei {0} zugeteilt."),
                    [5] = ("Turno assegnato",            "Sei stato assegnato a un turno presso {0}."),
                    [6] = ("Turno atribuído",            "Você foi atribuído a um turno em {0}.")
                },
                [5] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Exit Date Set",             "Your exit date at {0} has been scheduled."),
                    [2] = ("Date de sortie fixée",       "Votre date de sortie de {0} a été planifiée."),
                    [3] = ("Fecha de salida establecida","Tu fecha de salida en {0} ha sido programada."),
                    [4] = ("Austrittsdatum festgelegt",  "Ihr Austrittsdatum bei {0} wurde festgelegt."),
                    [5] = ("Data di uscita stabilita",   "La tua data di uscita da {0} è stata pianificata."),
                    [6] = ("Data de saída definida",     "A sua data de saída em {0} foi agendada.")
                }
            };

            var localizationSeeds = new List<NotificationTypeLocalization>();
            foreach (var (typeId, localizationDictionary) in notificationLocalizations)
            {
                foreach (var (localizationId, (displayValue, messageTemplate)) in localizationDictionary)
                {
                    localizationSeeds.Add(new NotificationTypeLocalization
                    {
                        NotificationTypeId = typeId,
                        LocalizationId = localizationId,
                        NotificationTypeDisplayValue = displayValue,
                        MessageTemplate = messageTemplate
                    });
                }
            }
            modelBuilder.Entity<NotificationTypeLocalization>().HasData(localizationSeeds);

            #endregion

            #region Subscription & Billing Configuration

            // SubscriptionPlanType
            modelBuilder.Entity<SubscriptionPlanType>()
                .HasKey(spt => spt.SubscriptionPlanTypeId);

            // SubscriptionPlanTypeLocalization
            modelBuilder.Entity<SubscriptionPlanTypeLocalization>()
                .HasKey(sptl => new { sptl.SubscriptionPlanTypeId, sptl.LocalizationId });

            modelBuilder.Entity<SubscriptionPlanTypeLocalization>()
                .HasOne(sptl => sptl.SubscriptionPlanType)
                .WithMany(spt => spt.SubscriptionPlanTypeLocalizations)
                .HasForeignKey(sptl => sptl.SubscriptionPlanTypeId);

            modelBuilder.Entity<SubscriptionPlanTypeLocalization>()
                .HasOne(sptl => sptl.Localization)
                .WithMany(l => l.SubscriptionPlanTypeLocalizations)
                .HasForeignKey(sptl => sptl.LocalizationId);

            // SubscriptionDurationType
            modelBuilder.Entity<SubscriptionDurationType>()
                .HasKey(sdt => sdt.SubscriptionDurationTypeId);

            // SubscriptionDurationTypeLocalization
            modelBuilder.Entity<SubscriptionDurationTypeLocalization>()
                .HasKey(sdtl => new { sdtl.SubscriptionDurationTypeId, sdtl.LocalizationId });

            modelBuilder.Entity<SubscriptionDurationTypeLocalization>()
                .HasOne(sdtl => sdtl.SubscriptionDurationType)
                .WithMany(sdt => sdt.SubscriptionDurationTypeLocalizations)
                .HasForeignKey(sdtl => sdtl.SubscriptionDurationTypeId);

            modelBuilder.Entity<SubscriptionDurationTypeLocalization>()
                .HasOne(sdtl => sdtl.Localization)
                .WithMany(l => l.SubscriptionDurationTypeLocalizations)
                .HasForeignKey(sdtl => sdtl.LocalizationId);

            // SubscriptionPlanDurationPrice — one row per tier+duration combination, priced/visible independently
            modelBuilder.Entity<SubscriptionPlanDurationPrice>()
                .HasKey(spdp => spdp.SubscriptionPlanDurationPriceId);

            modelBuilder.Entity<SubscriptionPlanDurationPrice>()
                .HasIndex(spdp => new { spdp.SubscriptionPlanTypeId, spdp.SubscriptionDurationTypeId })
                .IsUnique()
                .HasDatabaseName("IX_SubscriptionPlanDurationPrices_PlanType_DurationType");

            modelBuilder.Entity<SubscriptionPlanDurationPrice>()
                .HasOne(spdp => spdp.SubscriptionPlanType)
                .WithMany(spt => spt.SubscriptionPlanDurationPrices)
                .HasForeignKey(spdp => spdp.SubscriptionPlanTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscriptionPlanDurationPrice>()
                .HasOne(spdp => spdp.SubscriptionDurationType)
                .WithMany(sdt => sdt.SubscriptionPlanDurationPrices)
                .HasForeignKey(spdp => spdp.SubscriptionDurationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Campaign
            modelBuilder.Entity<Campaign>()
                .HasKey(c => c.CampaignId);

            // CampaignLocalization
            modelBuilder.Entity<CampaignLocalization>()
                .HasKey(cl => new { cl.CampaignId, cl.LocalizationId });

            modelBuilder.Entity<CampaignLocalization>()
                .HasOne(cl => cl.Campaign)
                .WithMany(c => c.CampaignLocalizations)
                .HasForeignKey(cl => cl.CampaignId);

            modelBuilder.Entity<CampaignLocalization>()
                .HasOne(cl => cl.Localization)
                .WithMany(l => l.CampaignLocalizations)
                .HasForeignKey(cl => cl.LocalizationId);

            // CampaignSubscriptionPlan — pure junction table (campaign <-> eligible plan+duration combination)
            modelBuilder.Entity<CampaignSubscriptionPlan>()
                .HasKey(csp => new { csp.CampaignId, csp.SubscriptionPlanDurationPriceId });

            modelBuilder.Entity<CampaignSubscriptionPlan>()
                .HasOne(csp => csp.Campaign)
                .WithMany(c => c.CampaignSchedulePlans)
                .HasForeignKey(csp => csp.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampaignSubscriptionPlan>()
                .HasOne(csp => csp.SubscriptionPlanDurationPrice)
                .WithMany(spdp => spdp.CampaignSubscriptionPlans)
                .HasForeignKey(csp => csp.SubscriptionPlanDurationPriceId)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentMethodType
            modelBuilder.Entity<PaymentMethodType>()
                .HasKey(pmt => pmt.PaymentMethodTypeId);

            // PaymentMethodTypeCountry
            modelBuilder.Entity<PaymentMethodTypeCountry>()
                .HasKey(pmtc => new { pmtc.PaymentMethodTypeId, pmtc.CountryCode });

            modelBuilder.Entity<PaymentMethodTypeCountry>()
                .HasOne(pmtc => pmtc.PaymentMethodType)
                .WithMany(pmt => pmt.PaymentMethodTypeCountries)
                .HasForeignKey(pmtc => pmtc.PaymentMethodTypeId);

            // PaymentMethodTypeLocalization
            modelBuilder.Entity<PaymentMethodTypeLocalization>()
                .HasKey(pmtl => new { pmtl.PaymentMethodTypeId, pmtl.LocalizationId });

            modelBuilder.Entity<PaymentMethodTypeLocalization>()
                .HasOne(pmtl => pmtl.PaymentMethodType)
                .WithMany(pmt => pmt.PaymentMethodTypeLocalizations)
                .HasForeignKey(pmtl => pmtl.PaymentMethodTypeId);

            modelBuilder.Entity<PaymentMethodTypeLocalization>()
                .HasOne(pmtl => pmtl.Localization)
                .WithMany(l => l.PaymentMethodTypeLocalizations)
                .HasForeignKey(pmtl => pmtl.LocalizationId);

            // PaymentMethod
            modelBuilder.Entity<PaymentMethod>()
                .HasKey(pm => pm.PaymentMethodId);

            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.Entity)
                .WithMany(e => e.PaymentMethods)
                .HasForeignKey(pm => pm.EntityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.PaymentMethodType)
                .WithMany(pmt => pmt.PaymentMethods)
                .HasForeignKey(pm => pm.PaymentMethodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // EntitySubscriptionPlan
            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasKey(esp => esp.EntitySubscriptionPlanId);

            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasOne(esp => esp.Entity)
                .WithMany(e => e.EntitySubscriptionPlans)
                .HasForeignKey(esp => esp.EntityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasOne(esp => esp.SubscriptionPlanDurationPrice)
                .WithMany(spdp => spdp.EntitySubscriptionPlans)
                .HasForeignKey(esp => esp.SubscriptionPlanDurationPriceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasOne(esp => esp.Campaign)
                .WithMany(c => c.EntitySubscriptionPlans)
                .HasForeignKey(esp => esp.CampaignId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasOne(esp => esp.PreviousSubscriptionPlan)
                .WithMany()
                .HasForeignKey(esp => esp.PreviousSubscriptionPlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // SubscriptionBillingRecord
            modelBuilder.Entity<SubscriptionBillingRecord>()
                .HasKey(sbr => sbr.SubscriptionBillingRecordId);

            modelBuilder.Entity<SubscriptionBillingRecord>()
                .HasOne(sbr => sbr.EntitySubscriptionPlan)
                .WithMany(esp => esp.SubscriptionBillingRecords)
                .HasForeignKey(sbr => sbr.EntitySubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // EntitySubscriptionPayment — Restrict on all FKs to avoid multiple cascade paths converging on EntitySubscriptionPlan
            modelBuilder.Entity<EntitySubscriptionPayment>()
                .HasKey(esp => esp.EntitySubscriptionPaymentId);

            modelBuilder.Entity<EntitySubscriptionPayment>()
                .HasOne(esp => esp.EntitySubscriptionPlan)
                .WithMany(esp => esp.EntitySubscriptionPayments)
                .HasForeignKey(esp => esp.EntitySubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntitySubscriptionPayment>()
                .HasOne(esp => esp.BillingRecord)
                .WithMany(sbr => sbr.EntitySubscriptionPayments)
                .HasForeignKey(esp => esp.BillingRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntitySubscriptionPayment>()
                .HasOne(esp => esp.PaymentMethod)
                .WithMany(pm => pm.EntitySubscriptionPayments)
                .HasForeignKey(esp => esp.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // ScheduleGeneration
            modelBuilder.Entity<ScheduleGeneration>()
                .HasKey(sg => sg.ScheduleGenerationId);

            modelBuilder.Entity<ScheduleGeneration>()
                .HasOne(sg => sg.EntitySubscriptionPlan)
                .WithMany(esp => esp.ScheduleGenerations)
                .HasForeignKey(sg => sg.EntitySubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentWebhookEvent — no required FK; events are stored before being matched/processed
            modelBuilder.Entity<PaymentWebhookEvent>()
                .HasKey(pwe => pwe.WebhookEventId);

            modelBuilder.Entity<PaymentWebhookEvent>()
                .HasIndex(pwe => pwe.GatewayEventId)
                .IsUnique()
                .HasDatabaseName("IX_PaymentWebhookEvents_GatewayEventId");

            modelBuilder.Entity<PaymentWebhookEvent>()
                .HasOne(pwe => pwe.EntitySubscriptionPayment)
                .WithMany(esp => esp.PaymentWebhookEvents)
                .HasForeignKey(pwe => pwe.EntitySubscriptionPaymentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // EntityBillingProfile
            modelBuilder.Entity<EntityBillingProfile>()
                .HasKey(ebp => ebp.EntityBillingProfileId);

            modelBuilder.Entity<EntityBillingProfile>()
                .HasOne(ebp => ebp.Entity)
                .WithMany(e => e.EntityBillingProfiles)
                .HasForeignKey(ebp => ebp.EntityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .HasKey(i => i.InvoiceId);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.SubscriptionBillingRecord)
                .WithMany(sbr => sbr.Invoices)
                .HasForeignKey(i => i.SubscriptionBillingRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.EntityBillingProfile)
                .WithMany(ebp => ebp.Invoices)
                .HasForeignKey(i => i.EntityBillingProfileId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes — main lookup columns queried by the billing/subscription flows
            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasIndex(esp => esp.EntityId)
                .HasDatabaseName("IX_EntitySubscriptionPlans_EntityId");

            modelBuilder.Entity<EntitySubscriptionPlan>()
                .HasIndex(esp => new { esp.EntityId, esp.Status })
                .HasDatabaseName("IX_EntitySubscriptionPlans_EntityId_Status");

            modelBuilder.Entity<PaymentMethod>()
                .HasIndex(pm => pm.EntityId)
                .HasDatabaseName("IX_PaymentMethods_EntityId");

            modelBuilder.Entity<EntityBillingProfile>()
                .HasIndex(ebp => ebp.EntityId)
                .HasDatabaseName("IX_EntityBillingProfiles_EntityId");

            modelBuilder.Entity<SubscriptionBillingRecord>()
                .HasIndex(sbr => sbr.EntitySubscriptionPlanId)
                .HasDatabaseName("IX_SubscriptionBillingRecords_EntitySubscriptionPlanId");

            modelBuilder.Entity<EntitySubscriptionPayment>()
                .HasIndex(esp => esp.BillingRecordId)
                .HasDatabaseName("IX_EntitySubscriptionPayments_BillingRecordId");

            modelBuilder.Entity<ScheduleGeneration>()
                .HasIndex(sg => sg.EntitySubscriptionPlanId)
                .HasDatabaseName("IX_ScheduleGenerations_EntitySubscriptionPlanId");

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.SubscriptionBillingRecordId)
                .HasDatabaseName("IX_Invoices_SubscriptionBillingRecordId");

            #endregion
        }

        #endregion

        private static LambdaExpression BuildSoftDeleteFilter(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var body = Expression.Not(Expression.Property(param, "IsDeleted"));
            return Expression.Lambda(body, param);
        }
    }
}
