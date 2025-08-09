namespace Kos.Models.DTO
{
    public class BookingDTO
    {
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int JumlahKamar { get; set; }
        public int JumlahTamu { get; set; }
        public string Status { get; set; } = "pending";
        public string RoomName { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();

    }
}
