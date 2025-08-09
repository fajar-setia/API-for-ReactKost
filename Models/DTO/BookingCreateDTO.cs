namespace Kos.Models.DTO
{
    public class BookingCreateDTO
    {
        public Guid RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public  string status { get; set; } = "pending";
        public int JumlahKamar { get; set; } // Default to 1 room
        public int JumlahTamu { get; set; }
    }
}
