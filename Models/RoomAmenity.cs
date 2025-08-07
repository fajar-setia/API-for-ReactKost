using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kos.Models
{
    public class RoomAmenity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relasi ke Room
        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; } = null!;
    }

}
