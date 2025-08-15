using System.ComponentModel.DataAnnotations;

namespace Kos.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoomId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int Rating { get; set; } // Assuming a rating scale of 1-5
        public string Comment { get; set; } = string.Empty;
        public bool IsAddressed { get; set; } = false;
        public string? AdminResponse { get; set; }  
        public string? ImageUrl { get; set; } 
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? ResponseDate { get; set; }
        // Navigation property
        public Room Room { get; set; } = null!;
    }
}
