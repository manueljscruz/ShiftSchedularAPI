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

        }

        #endregion
    }
}
