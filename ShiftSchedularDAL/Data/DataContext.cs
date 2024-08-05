using Microsoft.EntityFrameworkCore;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #region On Configuring

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies();
        //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-T0HQHVC\\SQLEXPRESS;Database=ShiftSchedular; Integrated Security=True; Trusted_Connection=True; Trust Server Certificate=False; MultipleActiveResultSets=True; Encrypt=False");
        //    // optionsBuilder.UseSqlServer("Data Source=DESKTOP-L6HTG1E\\SQLEXPRESS;Database=ShiftSchedular; Integrated Security=True; Trusted_Connection=True; Trust Server Certificate=False; MultipleActiveResultSets=True; Encrypt=False");

        //}

        #endregion

        #region Db Sets

        public DbSet<Gender> Genders { get; set; }
        public DbSet<GenderLocalization> GenderLocalizations { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityWorker> EntityWorkers { get; set; }
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

        #endregion

        #region On Model Creating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Gender Configuration

            modelBuilder.Entity<Gender>()
                .HasKey(g => g.GenderId);

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

            #region Worker Configuration

            modelBuilder.Entity<Worker>()
                .HasKey(w => w.WorkerId);

            // Configure relationship with Gender
            modelBuilder.Entity<Worker>()
                .HasOne(w=>w.Gender)
                .WithMany(g=>g.Workers)
                .HasForeignKey(w=>w.GenderId);

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
                .HasKey(ew => new { ew.EntityId, ew.WorkerId, ew.SkillId });

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.Entity)
                .WithMany(e => e.EntityWorkers)
                .HasForeignKey(ew => ew.EntityId);

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.Worker)
                .WithMany(e => e.EntityWorkers)
                .HasForeignKey(ew => ew.WorkerId);

            modelBuilder.Entity<EntityWorker>()
                .HasOne(ew => ew.Skill)
                .WithMany(s => s.EntityWorkers)
                .HasForeignKey(ew => ew.SkillId)
                .IsRequired(false);

            #endregion

            #region Entity Worker Invitation Configuration

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasKey(ewi => new { ewi.EntityId, ewi.Email });

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasOne(ewi => ewi.Entity)
                .WithMany(e => e.EntityWorkerInvitations)
                .HasForeignKey(ewi => ewi.EntityId);

            modelBuilder.Entity<EntityWorkerInvitation>()
                .HasOne(ewi => ewi.Worker)
                .WithMany(w => w.EntityWorkerInvitations)
                .HasForeignKey(ewi => ewi.WorkerId);

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
                .HasOne(ewa => ewa.Worker)
                .WithMany(w => w.EntityWorkerAbsences)
                .HasForeignKey(ewa => ewa.WorkerId);

            modelBuilder.Entity<EntityWorkerAbsence>()
                .HasOne(ewa => ewa.Entity)
                .WithMany(e => e.EntityWorkerAbsences)
                .HasForeignKey(ewa => ewa.EntityId);

            #endregion

            #region Schedule Entry Configuration

            modelBuilder.Entity<ScheduleEntry>()
                .HasKey(se => se.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(se => se.Shift)
                .WithMany(s => s.ScheduleEntries)
                .HasForeignKey(se => se.ShiftId);

            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(se => se.Entity)
                .WithMany(e => e.ScheduleEntries)
                .HasForeignKey(se => se.EntityId);

            #endregion

            #region Schedule Entry Workers Configuration

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasKey(sew => new { sew.ScheduleEntryId, sew.WorkerId });

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasOne(sew => sew.ScheduleEntry)
                .WithMany(se => se.ScheduleEntryWorkers)
                .HasForeignKey(sew => sew.ScheduleEntryId);

            modelBuilder.Entity<ScheduleEntryWorkers>()
                .HasOne(sew => sew.Worker)
                .WithMany(w => w.ScheduleEntryWorkers)
                .HasForeignKey(sew => sew.WorkerId);

            #endregion
        }

        #endregion
    }
}
