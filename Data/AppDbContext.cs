using Microsoft.EntityFrameworkCore;

namespace Kos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Room> Rooms { get; set; } = null!;
        public DbSet<Models.Booking> Bookings { get; set; } = null!;
        public DbSet<Models.Review> Reviews { get; set; } = null!;
        public DbSet<Models.Gallery> Galleries { get; set; } = null!;
        public DbSet<Models.RoomType> RoomTypes { get; set; } = null!;
        public DbSet<Models.RoomAmenity> RoomAmenities { get; set; } = null!;

    }
}
