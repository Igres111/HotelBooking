using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Repositories.ReviewRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            await _methods.RegisterReview(review);
            return Ok("Review Added");
        }
    }
}
