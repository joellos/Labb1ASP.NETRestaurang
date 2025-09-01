using Labb1ASP.NETDatabas.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        // DbSets - representerar tabellerna i databasen
        public DbSet<Table> Tables { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurera Table entity
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TableNumber).IsRequired();
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Unique constraint - inget dubbelt bordsnummer
                entity.HasIndex(e => e.TableNumber).IsUnique();
            });

            // Konfigurera Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Index för snabbare sökning på telefonnummer och email
                entity.HasIndex(e => e.PhoneNumber);
                entity.HasIndex(e => e.Email);
            });

            // Konfigurera Administrator entity
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Konfigurera Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookingDateTime).IsRequired();
                entity.Property(e => e.NumberOfGuests).IsRequired();
                entity.Property(e => e.SpecialRequests).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Foreign Key relationships
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Bookings)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                entity.HasOne(e => e.Table)
                      .WithMany(t => t.Bookings)
                      .HasForeignKey(e => e.TableId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Index för snabbare queries på datum och bord
                entity.HasIndex(e => e.BookingDateTime);
                entity.HasIndex(e => new { e.TableId, e.BookingDateTime });
            });

            // Konfigurera MenuItem entity
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsPopular).HasDefaultValue(false);
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Index för snabbare sökning på kategori och tillgänglighet
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsAvailable);
                entity.HasIndex(e => e.IsPopular);
            });
        }
    }
}
