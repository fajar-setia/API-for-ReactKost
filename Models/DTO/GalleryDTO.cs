namespace Kos.Models.DTO
{
    public class GalleryDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public Guid? RoomId { get; set; }
        public string? RoomName { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
