using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Kos.Data;
using Kos.Models; // Ensure this namespace matches your project structure
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
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            review.Id = Guid.NewGuid(); // Ensure a new ID is generated
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReviews), new { id = review.Id }, review);
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
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByRoom(int roomId)
        {
            var reviews = await _context.Reviews.Where(r => r.RoomId == roomId).ToListAsync();
            if (reviews == null || !reviews.Any())
            {
                return NotFound("No reviews found for this room.");
            }
            return Ok(reviews);

        }
        [HttpGet("average/{roomId}")]
        public async Task<IActionResult> GetAverageRating(int roomId)
        {
            var avg = await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
            return Ok(avg);
        }
    }
}
