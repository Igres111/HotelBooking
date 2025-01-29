using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;
using HotelBooking.Services;
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
        public readonly ITokenGenerator _tokenGenerator;
        public UserController(Context context, IMapper mapper, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var found = _context.Users.FirstOrDefault(entity => entity.Email == user.Email);
            if(found != null)
            {
                return BadRequest("User already exists");
            }
            User newUser = _mapper.Map<User>(user);
            newUser.Id = Guid.NewGuid();
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Register(LoginDto user)
        {
            var result = _context
                .Users
                .FirstOrDefault(entity => entity.Email == user.Email && entity.Password == user.Password);
            if (result == null)
            {
                return NotFound();
            }
           var refreshToken= await _tokenGenerator.CreateRefreshTokenAsync(result);
           var accessToken = _tokenGenerator.CreateAccessToken(result);
           await _context.SaveChangesAsync();
           return Ok(new { result.Id, accessToken, refreshToken.Token });
        }
        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var result = await _tokenGenerator.RefreshAccessTokenAsync(token);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id,UpdateUserDto newUser)
        {
            var found = await _context.Users.FirstOrDefaultAsync(entity => entity.Id == id);
            if(found == null)
            {
                return NotFound();
            }
            found = _mapper.Map(newUser,found);
            await _context.SaveChangesAsync();
            return Ok("Profile Updated");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id)
        {
            var found = await _context.Users.FirstOrDefaultAsync(entity => entity.Id == id);
            if (found == null)
            {
                return NotFound();
            }
            _context.Users.Remove(found);
            await _context.SaveChangesAsync();
            return Ok("Profile Deleted");
        }
        }
}
