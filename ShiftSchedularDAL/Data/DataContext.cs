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
        }

        #endregion
    }
}
