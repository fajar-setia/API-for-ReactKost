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
            var bookings = await _context.Bookings.Include(b => b.Room).ThenInclude(b => b.Amenities).Select(b => new BookingDTO
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalPrice = b.TotalPrice,
                RoomName = b.Room.Name,
                Amenities = b.Room.Amenities.Select(a => a.Name).ToList(),
                Status = b.Status,
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
                ((b.StartDate < dto.EndDate && b.EndDate > dto.StartDate) ||
                 (b.StartDate == dto.StartDate && b.EndDate == dto.EndDate)));

            if (conflict)
            {
                return Conflict("Tanggal sudah dibooking.");
            }

            var totalDays = (dto.EndDate - dto.StartDate).Days;
            var totalPrice = totalDays * room.PricePerNight * dto.JumlahKamar;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                TotalPrice = totalPrice,
                JumlahKamar = dto.JumlahKamar,
                JumlahTamu = dto.JumlahTamu,
                Status = "pending"
            };

            _context.Bookings.Add(booking);
            room.IsAvailable = false;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, new BookingDTO
            {
                Id = booking.Id,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                RoomName = room.Name,
                JumlahKamar = booking.JumlahKamar,
                JumlahTamu = booking.JumlahTamu,
                Status = booking.Status,
            });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            var booking = await _context.Bookings
                          .Include(b => b.Room)
                          .ThenInclude(b => b.Amenities)
                          .FirstOrDefaultAsync(b => b.Id == id);
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
            existingBooking.JumlahKamar = booking.JumlahKamar;
            existingBooking.JumlahTamu = booking.JumlahTamu;
            existingBooking.Status = booking.Status;
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

            // Ambil room yang bersangkutan
            var room = await _context.Rooms.FindAsync(booking.RoomId);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            // Cek jika tidak ada booking aktif untuk room ini, ubah jadi tersedia
            bool stillHasBooking = await _context.Bookings.AnyAsync(b => b.RoomId == booking.RoomId);
            if (!stillHasBooking && room != null)
            {
                room.IsAvailable = true;
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByRoom(Guid roomId)
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
                RoomName = b.Room.Name,
                Status = b.Status,
            }).ToListAsync();
            return Ok(bookings);
        }
        [HttpGet("available")]
        public async Task<IActionResult> CheckAvailability(string roomId, string startDate, string endDate)
        {
            if (!Guid.TryParse(roomId, out var roomGuid))
                return BadRequest("Invalid roomId");

            if (!DateTime.TryParse(startDate, out var start))
                return BadRequest("Invalid startDate");

            if (!DateTime.TryParse(endDate, out var end))
                return BadRequest("Invalid endDate");

            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            // Lanjut ke cek booking seperti biasa
            var room = await _context.Rooms.FindAsync(roomGuid);
            if (room == null)
                return NotFound("Room not found");

            bool isBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomGuid &&
                ((start >= b.StartDate && start < b.EndDate) ||
                 (end > b.StartDate && end <= b.EndDate) ||
                 (start <= b.StartDate && end >= b.EndDate))
            );

            return Ok(new { available = !isBooked });
        }





    }
}
