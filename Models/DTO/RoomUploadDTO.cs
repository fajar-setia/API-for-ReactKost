namespace Kos.Models.DTO
{
    public class RoomUploadDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }
        public IFormFile? MainImage { get; set; }
        public int? RoomTypeId { get; set; } // Foreign key for RoomType
        public RoomType? RoomType { get; set; } // Navigation property for RoomType
        public List<string> Amenities { get; set; } = new();
    }
}
