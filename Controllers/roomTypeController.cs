using Kos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kos.Models; // Ensure this namespace matches your project structure

namespace Kos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class roomTypeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public roomTypeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomTypes()
        {
            var roomTypes = await _context.RoomTypes
                .Include(rt => rt.Rooms) // <-- ini penting!
                .Select(rt => new {
                    roomTypeId = rt.RoomTypeId,
                    name = rt.Name,
                    rooms = rt.Rooms.Select(r => new {
                        r.Id,
                        r.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(roomTypes);
        }

        [HttpPost]
        public async Task<ActionResult<RoomType>> CreateRoomType(RoomType roomType)
        {
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoomTypes), new { id = roomType.RoomTypeId }, roomType);
        }
    }
    
        
    
}
