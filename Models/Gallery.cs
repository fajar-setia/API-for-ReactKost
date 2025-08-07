using System.ComponentModel.DataAnnotations;

namespace Kos.Models
{
    public class Gallery
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public Guid? RoomId { get; set; }
        public string Category { get; set; } = "rooms"; // default atau enum
        public bool Featured { get; set; } = false;

        // Navigation property
        public Room? Room { get; set; } = null!;
    }
}
