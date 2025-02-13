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
        [HttpPost("Book-Hotel")]
        public async Task<IActionResult> BookingHotel(BookingInfoDto info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           var result = await _methods.BookHotel(info);
            return Ok(result);
        }
    }
}
