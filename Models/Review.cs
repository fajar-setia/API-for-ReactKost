using System.ComponentModel.DataAnnotations;

namespace Kos.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int RoomId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int Rating { get; set; } // Assuming a rating scale of 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        // Navigation property
        public Room Room { get; set; } = null!;
    }
}
