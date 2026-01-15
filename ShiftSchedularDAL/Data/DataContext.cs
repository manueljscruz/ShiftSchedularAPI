using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public override void Dispose()
        {
            Console.WriteLine("🚨 DataContext is being disposed!");
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            Console.WriteLine("🚨 DataContext is being disposed asynchronously!");
            return base.DisposeAsync();
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

        #endregion

        #region On Model Creating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            #region Database Indexes for Performance

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

            #endregion
        }

        #endregion
    }
}
