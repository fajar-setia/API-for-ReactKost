using Kos.Data;
using Kos.Models;
using Kos.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoomController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Gallery)
                .Include(r => r.Reviews)
                .Include(r => r.Amenities)
                .Include(r => r.RoomType)
                .ToListAsync();
            return Ok(rooms);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromForm] RoomUploadDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Room cannot be null.");
            }

            string ImageUrl = string.Empty;

            // Simpan gambar utama jika ada
            if (dto.MainImage != null && dto.MainImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "room");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.MainImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.MainImage.CopyToAsync(stream);
                }

                ImageUrl = $"/uploads/room/{fileName}";
            }

            // Buat objek Room
            var room = new Models.Room
            {
                Name = dto.Name,
                Description = dto.Description,
                Capacity = dto.Capacity,
                IsAvailable = dto.IsAvailable,
                PricePerNight = dto.PricePerNight,
                MainImageUrl = ImageUrl,
                RoomTypeId = dto.RoomTypeId
            };

            // Simpan Room dulu agar Id tersedia untuk relasi
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            // Simpan amenities ke tabel RoomAmenities
            if (dto.Amenities != null && dto.Amenities.Any())
            {
                foreach (var amenity in dto.Amenities)
                {
                    var roomAmenity = new RoomAmenity
                    {
                        RoomId = room.Id,
                        Name = amenity
                    };
                    _context.RoomAmenities.Add(roomAmenity);
                }
                await _context.SaveChangesAsync(); // simpan amenity
            }

            // Ambil room yang sudah lengkap dengan amenities untuk dikembalikan
            var result = await _context.Rooms
                .Include(r => r.Amenities)
                .FirstOrDefaultAsync(r => r.Id == room.Id);

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(Guid id)
        {
            var room = await _context.Rooms.Include(r => r.Gallery).Include(r => r.Reviews).Include(r => r.Amenities).FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] Models.Room room)
        {
            if (id != room.Id)
            {
                return BadRequest("Room ID mismatch.");
            }
            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }
            existingRoom.Name = room.Name;
            existingRoom.Description = room.Description;
            existingRoom.Capacity = room.Capacity;
            existingRoom.IsAvailable = room.IsAvailable;
            existingRoom.PricePerNight = room.PricePerNight;
            existingRoom.MainImageUrl = room.MainImageUrl;
            _context.Entry(existingRoom).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var rooms = await _context.Rooms
                .Where(r => r.IsAvailable)
                .Include(r => r.RoomType)
                .ToListAsync();
            return Ok(rooms);
        }
    }
}
