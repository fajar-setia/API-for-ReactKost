using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Kos.Data;
using Kos.Models; // Ensure this namespace matches your project structure
using Kos.Models.DTO; // Ensure this namespace matches your project structure
namespace Kos.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class reviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        public reviewController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            var reviews = await _context.Reviews.Include(r => r.Room).ToListAsync();
            return Ok(reviews);
        }
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview([FromForm] ReviewCreateDto reviewDto)
        {
            string? imageUrl = null;

            if (reviewDto.Image != null && reviewDto.Image.Length > 0)
            {
                // Path simpan gambar
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(reviewDto.Image.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await reviewDto.Image.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/reviews/{fileName}";
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                RoomId = reviewDto.RoomId,
                CustomerName = reviewDto.CustomerName,
                CustomerEmail = reviewDto.CustomerEmail,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                ImageUrl = imageUrl,
                DateCreated = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(Guid id)
        {
            var review = await _context.Reviews.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest("Review ID mismatch.");
            }
            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }
            existingReview.RoomId = review.RoomId;
            existingReview.CustomerName = review.CustomerName;
            existingReview.CustomerEmail = review.CustomerEmail;
            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.DateCreated = review.DateCreated;
            existingReview.IsAddressed = review.IsAddressed;
            existingReview.AdminResponse = review.AdminResponse;
            _context.Entry(existingReview).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByRoom(Guid roomId)
        {
            var reviews = await _context.Reviews.Where(r => r.RoomId == roomId).ToListAsync();
            if (reviews == null || !reviews.Any())
            {
                return NotFound("No reviews found for this room.");
            }
            return Ok(reviews);

        }
        [HttpGet("average/{roomId}")]
        public async Task<IActionResult> GetAverageRating(Guid roomId)
        {
            var avg = await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
            return Ok(avg);
        }
        [HttpPatch("respond/{id}")]
        public async Task<IActionResult> RespondToReview(Guid id, [FromBody] ReviewResponseDto responseDto)
        {
            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            existingReview.IsAddressed = responseDto.IsAddressed;
            existingReview.AdminResponse = responseDto.AdminResponse;

            _context.Entry(existingReview).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
