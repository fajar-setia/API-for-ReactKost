using System.ComponentModel.DataAnnotations;

namespace Kos.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
