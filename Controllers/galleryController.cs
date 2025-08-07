using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Kos.Data;
using Kos.Models;
using Kos.Models.DTO;
namespace Kos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class galleryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public galleryController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetGalleryByRoomId(Guid roomId)
        {
            return Ok(await _context.Galleries
                .Where(g => g.RoomId == roomId)
                .ToListAsync());

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gallery>>> GetAllGalleries()
        {
            var galleries = await _context.Galleries.Include(g => g.Room).Select(g => new GalleryDTO
            {
                Id = g.Id,
                Title = g.Title,
                ImageUrl = g.ImageUrl,
                Description = g.Description,
                DateAdded = g.DateAdded,
                RoomId = g.RoomId,
                RoomName = g.Room.Name,
                Category = g.Category,
            }).ToListAsync();
            return Ok(galleries);
        }

        [HttpPost]
        public async Task<ActionResult<Gallery>> PostGallery(Gallery gallery)
        {
            if (gallery == null)
            {
                return BadRequest("Gallery cannot be null.");
            }
            gallery.Id = Guid.NewGuid(); // Ensure a new ID is generated
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGalleryByRoomId), new { id = gallery.Id }, gallery);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Gallery>> GetGallery(Guid id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null)
            {
                return NotFound();
            }
            return Ok(gallery);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGallery(Guid id, Gallery gallery)
        {
            if (id != gallery.Id)
            {
                return BadRequest("Gallery ID mismatch.");
            }
            var existingGallery = await _context.Galleries.FindAsync(id);
            if (existingGallery == null)
            {
                return NotFound();
            }
            existingGallery.Title = gallery.Title;
            existingGallery.ImageUrl = gallery.ImageUrl;
            existingGallery.Description = gallery.Description;
            existingGallery.DateAdded = gallery.DateAdded;
            existingGallery.RoomId = gallery.RoomId;
            existingGallery.Category = gallery.Category;
            _context.Entry(existingGallery).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGallery(Guid id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null)
            {
                return NotFound();
            }
            _context.Galleries.Remove(gallery);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadGallery([FromForm] GalleryUploadDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // tambahkan ini
            }
            if (dto == null || dto.ImageFile == null || dto.ImageFile.Length == 0)
            {
                return BadRequest("Invalid gallery upload data.");
            }
           
            if (dto.Category == "rooms" && dto.RoomId == Guid.Empty)
            {
                return BadRequest("RoomId wajib untuk kategori rooms");
            }
            if (dto.Category != "rooms")
            {
                dto.RoomId = null;
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "gallery");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }
            var gallery = new Gallery
            {
                Id = Guid.NewGuid(),
                Title = dto.Title ?? "",
                RoomId = dto.RoomId,
                ImageUrl = $"/uploads/gallery/{fileName}",
                Description = dto.Description,
                DateAdded = DateTime.UtcNow,
                Category = dto.Category,
            };
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();
            return Ok(gallery);

        }
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Gallery>>> GetByCategory(string category)
        {
            return Ok(await _context.Galleries
                .Where(g => g.Category == category)
                .ToListAsync());
        }

    }

}
