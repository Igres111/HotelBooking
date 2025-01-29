using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public UserController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            User newUser = _mapper.Map<User>(user);
            newUser.Id = Guid.NewGuid();
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
