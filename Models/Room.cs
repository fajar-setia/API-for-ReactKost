using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kos.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }
        public string? MainImageUrl { get; set; }
        public int? RoomTypeId { get; set; } // Foreign key for RoomType
        public RoomType? RoomType { get; set; } // Navigation property for RoomType
        // Navigation properties can be added here if needed
        public ICollection<Booking> Bookings { get; set;  } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Gallery> Gallery { get; set; } = new List<Gallery>();
        public ICollection<RoomAmenity> Amenities { get; set; } = new List<RoomAmenity>();

    }
}
