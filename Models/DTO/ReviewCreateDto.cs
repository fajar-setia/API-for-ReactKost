using System.ComponentModel.DataAnnotations;

namespace Kos.Models.DTO
{
    public class ReviewCreateDto
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        // File upload
        public IFormFile? Image { get; set; }
    }
}
