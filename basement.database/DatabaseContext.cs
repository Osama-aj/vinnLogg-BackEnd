using basement.core.Entities.GeneralSection;
using basement.core.Entities.UserSection;
using Microsoft.EntityFrameworkCore;

namespace basement.database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<District> Districts { get; set; }

        public DbSet<Grape> Grapes { get; set; }

        public DbSet<Wine> Wines { get; set; }
        public DbSet<WineGrape> WineGrapes { get; set; }
        public DbSet<Vintage> Vintages { get; set; }

        /////////////////////////////

        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GradeTable> Grades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID=postgres;Password=1234;Host=localhost;Port=5432;Database=wdb;");
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();
            
            modelBuilder.Entity<Wine>()
                .HasIndex(u => u.Name)
                .IsUnique();

      


            modelBuilder.Entity<WineGrape>()
                .HasKey(wg => new {wg.WineId, wg.GrapeId});
            modelBuilder.Entity<WineGrape>()
                .HasOne(wg => wg.Wine)
                .WithMany(w => w.WineGrapes)
                .HasForeignKey(wg => wg.WineId);
            modelBuilder.Entity<WineGrape>()
                .HasOne(wg => wg.Grape)
                .WithMany(g => g.WineGrapes)
                .HasForeignKey(wg => wg.GrapeId);

            base.OnModelCreating(modelBuilder);

        }
    }
}