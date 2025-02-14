using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Repositories.HotelRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

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
            try
            {
                var result = await _methods.GetHotelsList();
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound("Hotels Not Found");
            }
        }
        [HttpPost("Register-Hotel")]
        public async Task<IActionResult> Register(RegisterHotelDto hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _methods.RegisterHotel(hotel);
                return Ok("Registered Succesfully");
            }
            catch (Exception)
            {
                return  BadRequest("Hotel not registered");
            }         
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelWithId(Guid id)
        {
            try
            {
                var result = await _methods.GetHotelById(id);
                return Ok(result);
            }
            catch (Exception)
            {
              return NotFound("Hotel not found");
            }
        }
        [HttpPut("Hotel/{id}")]
        public async Task<IActionResult> UpdateHotel(Guid id, HotelChangesDto hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _methods.UpdateHotel(id, hotel);
                return Ok("Hotel Updated");
            }
            catch (Exception)
            {
                return BadRequest("Hotel not updated");
            }
        }
        [HttpDelete("Hotel/{id}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            try
            {
                await _methods.DeleteHotel(id);
                return Ok("Hotel Deleted");
            }
            catch (Exception)
            {
                return BadRequest("Hotel not deleted");
            }
        }
    }
}
