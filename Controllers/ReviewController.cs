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
        public ReviewController(IReviewRepository methods)
        {
            _methods = methods;
        }
        [HttpPost("add-review")]
        public async Task<IActionResult> PostReview(RegisterReviewDto review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _methods.RegisterReview(review);
                return Ok("Review Added");
            }
            catch (Exception)
            {
                return BadRequest("Review not added");
            }
  
        }
        [HttpGet("reviews-by-id")]
        public async Task<IActionResult> GetReviewById(Guid hotelId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _methods.GetReviewsById(hotelId);
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Reviews not found");
            }
        }
        [HttpDelete("delete-review")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                await _methods.DeleteReview(reviewId);
                return Ok("Review Deleted");
            }
            catch (Exception)
            {
                return NotFound("Review not found");
            }
        }
    }
}
