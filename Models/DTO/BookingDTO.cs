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
        public string RoomName { get; set; } = string.Empty;
    }
}
