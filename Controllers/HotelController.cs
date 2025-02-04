using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Repositories.HotelRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public readonly IHotelRepository _methods;
        public HotelController(Context context, IMapper mapper,IHotelRepository methods)
        {
            _context = context;
            _mapper = mapper;
            _methods = methods;
        }
        [HttpPost("RegisterHotel")]
        public async Task<IActionResult> Register(RegisterHotelDto hotel)
        {
            await _methods.RegisterHotel(hotel);
            return Ok("Registered Succesfully");
        }
    }
}
