namespace Kos.Models.DTO
{
    public class GalleryUploadDTO
    {
        public string? Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } = null;
        public Guid? RoomId { get; set; }
        public string Category { get; set; } = "rooms";
    }
}
