using System.ComponentModel.DataAnnotations;

namespace Kos.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int JumlahKamar { get; set; }
        public int JumlahTamu { get; set; }
        public string Status { get; set; } = "pending";
        // Navigation property
        public Room Room { get; set; } = null!;
    }
}
