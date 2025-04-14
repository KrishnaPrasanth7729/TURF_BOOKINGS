using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TURF_BOOKINGS.Models;

namespace TURF_BOOKINGS.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }

        public DbSet<RegisterModel> User { get; set; }
        public DbSet<BookingModel> Booking { get; set; }
        public DbSet<reviewModel> Review { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingModel>()
                .Property(b => b.TimeSlots)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
