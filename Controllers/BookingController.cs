using HotelBooking.DTOs.BookingDtos;
using HotelBooking.Repositories.BookingRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public readonly IBookingRepository _methods;
        public BookingController(IBookingRepository methods)
        {
            _methods = methods;
        }
        [HttpPost("Booking")]
        public async Task<IActionResult> BookingHotel(BookingInfoDto info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           var result = await _methods.BookHotel(info);
            return Ok(result);
        }
        [HttpGet("Bookings/{id}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            try
            {
                var result = await _methods.GetBookingById(id);
                return Ok(result);
            }
            catch (Exception)
            {
               return NotFound("Booking not found");
            }
        }
        [HttpGet("Bookings/{userId}")]
        public async Task<IActionResult> BookingByUsers(Guid userId)
        {
            try
            {
                var result = await _methods.GetBookingByUser(userId);
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Booking not found");
            }
        }
    }
}
