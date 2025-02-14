using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;
using HotelBooking.Repositories.UserRepo;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
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
            try
            {
                await _methods.RegisterUser(user);
                return Created();
            }
            catch (Exception)
            {

                return BadRequest("Email already exists");
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _methods.LoginUser(user);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Invalid Email or Password");
            }

        }
        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            try
            {
                var result = await _tokenGenerator.RefreshAccessTokenAsync(token);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Invalid Token");
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,UpdateUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _methods.UpdateUser(id, newUser);
                return Ok("Profile Updated");
            }
            catch (Exception)
            {
                return BadRequest("Invalid Credentials");
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _methods.DeleteUser(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound("User not found to delete");
            }
        }
        }
}
