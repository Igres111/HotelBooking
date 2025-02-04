using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;
using HotelBooking.Repositories.UserRepo;
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
        public readonly ITokenGenerator _tokenGenerator;
        public readonly IUserRepository _methods;
        public UserController(ITokenGenerator tokenGenerator, IUserRepository methods)
        {
            _tokenGenerator = tokenGenerator;
            _methods = methods;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _methods.ReceiveUsers();
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _methods.RegisterUser(user);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _methods.LoginUser(user);
            return Ok(result);
        }
        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var result = await _tokenGenerator.RefreshAccessTokenAsync(token);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,UpdateUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _methods.UpdateUser(id, newUser);
            return Ok("Profile Updated");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _methods.DeleteUser(id);
            return Ok("Profile Deleted");
        }
        }
}
