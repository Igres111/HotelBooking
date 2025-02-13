using HotelBooking.Data;
using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Repositories.ReviewRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        public readonly IReviewRepository _methods;
        public readonly Context _context;
        public ReviewController(IReviewRepository methods, Context context)
        {
            _methods = methods;
            _context = context;
        }
        [HttpPost("add-review")]
        public async Task<IActionResult> PostReview(RegisterReviewDto review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _methods.RegisterReview(review);
            return Ok("Review Added");
        }
        [HttpGet("reviews-by-id")]
        public async Task<IActionResult> GetReviewById(Guid hotelId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _methods.GetReviewsById(hotelId);

            return Ok(result);
        }
    }
}
