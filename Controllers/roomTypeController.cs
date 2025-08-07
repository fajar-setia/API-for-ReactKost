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
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            return Ok(await _context.RoomTypes.ToListAsync());
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
