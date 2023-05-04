using Microsoft.EntityFrameworkCore;

namespace WorldCitiesAPI.Data.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext() : base() 
        { 
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<City>(e =>
            {
                e.ToTable("Cities");

                e.HasKey(p => p.Id);

                e.Property(p=>p.Id)
                    .IsRequired();

                e.Property(p => p.Lat)
                    .HasColumnType("decimal(7,4)");

                e.Property(p => p.Lon)
                    .HasColumnType("decimal(7,4)");
            });

            modelBuilder.Entity<Country>(e =>
            {
                e.ToTable("Countries");

                e.HasKey(p => p.Id);

                e.Property(p => p.Id)
                    .IsRequired();
            });

            modelBuilder.Entity<City>()
                .HasOne(p => p.Country)
                .WithMany(p => p.Cities)
                .HasForeignKey(p => p.CountryId);
        }
    }
}
