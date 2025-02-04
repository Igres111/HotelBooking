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
        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =await _methods.GetHotelsList();
            return Ok("Hotels List");
        }
        [HttpPost("Register-Hotel")]
        public async Task<IActionResult> Register(RegisterHotelDto hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _methods.RegisterHotel(hotel);
            return Ok("Registered Succesfully");
        }
    }
}
