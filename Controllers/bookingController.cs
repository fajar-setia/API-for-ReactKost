using Microsoft.AspNetCore.Mvc;
using Kos.Data;
using Microsoft.EntityFrameworkCore;
using Kos.Models;
using Kos.Models.DTO; // Ensure this namespace matches your project structure

namespace Kos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class bookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public bookingController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _context.Bookings.Include(b => b.Room).Select(b => new BookingDTO
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalPrice = b.TotalPrice,
                RoomName = b.Room.Name
            }).ToListAsync();
            return Ok(bookings);
        }
        [HttpPost]
        public async Task<ActionResult<BookingDTO>> PostBooking(BookingCreateDTO dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            var conflict = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                ((b.StartDate < dto.EndDate && b.EndDate > dto.StartDate) || // Overlapping dates
                 (b.StartDate == dto.StartDate && b.EndDate == dto.EndDate))); // Exact match
            if (conflict)
            {
                return Conflict("tanggal sudah dibooking.");
            }
            var totalDays = (dto.EndDate - dto.StartDate).Days;
            var totalPrice = totalDays * room.PricePerNight; // Calculate total price based on room price per night
            var Booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                TotalPrice = totalPrice
            };
            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooking), new { id = Booking.Id}, new BookingDTO
            {
                Id = Booking.Id,
                CustomerName = Booking.CustomerName,
                CustomerEmail = Booking.CustomerEmail,
                CustomerPhone = Booking.CustomerPhone,
                StartDate = Booking.StartDate,
                EndDate = Booking.EndDate,
                TotalPrice = Booking.TotalPrice,
                RoomName = room.Name
            });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            var booking = await _context.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest("Booking ID mismatch.");
            }
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }
            existingBooking.RoomId = booking.RoomId;
            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.CustomerName = booking.CustomerName;
            existingBooking.CustomerEmail = booking.CustomerEmail;
            existingBooking.CustomerPhone = booking.CustomerPhone;
            existingBooking.TotalPrice = booking.TotalPrice;
            _context.Entry(existingBooking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByRoom(int roomId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .Include(b => b.Room)
                .ToListAsync();
            return Ok(bookings);

        }
        [HttpGet("with-room")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookingsWithRoom()
        {
            var bookings = await _context.Bookings.Include(b => b.Room).Select(b => new BookingDTO
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalPrice = b.TotalPrice,
                RoomName = b.Room.Name
            }).ToListAsync();
            return Ok(bookings);
        }
    }
}
