
using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotelBooking.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public readonly ITokenGenerator _tokenGenerator;
        public UserRepository(Context context, IMapper mapper, ITokenGenerator tokenGenerator)
        {
            _context = context;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<List<ReceiveUserDto>> ReceiveUsers()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<ReceiveUserDto>>(users);
        }

        public async Task RegisterUser(RegisterDto user)
        {
            var found = _context.Users.FirstOrDefault(entity => entity.Email == user.Email);
            if (found != null)
            {
                throw new Exception("User already exists");
            }
            User newUser = _mapper.Map<User>(user);
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            newUser.Id = Guid.NewGuid();
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }
        public async Task<ReceiveTokenDto> LoginUser(LoginDto user)
        {
            var result = _context
               .Users
               .FirstOrDefault(entity => entity.Email == user.Email);
            if (result == null)
            {
                throw new Exception("User not found");
            }
            if (user.Email != null && BCrypt.Net.BCrypt.Verify(user.Password, result.Password)) { 
                var tokenExist = _context.RefreshTokens.FirstOrDefault(entity => entity.UserId == result.Id);
                var refreshToken = await _tokenGenerator.CreateRefreshTokenAsync(result);
                if (tokenExist != null)
                {
                    _context.RefreshTokens.Remove(tokenExist);
                }
            var accessToken = _tokenGenerator.CreateAccessToken(result);
            await _context.SaveChangesAsync();
            return new ReceiveTokenDto { Id = result.Id, AccsessToken = accessToken, RefreshToken = refreshToken.Token };
            }
            throw new Exception("Invalid email or password");
        }
        public async Task UpdateUser(Guid id, UpdateUserDto newUser)
        {
            var found = await _context.Users.FirstOrDefaultAsync(entity => entity.Id == id);
            if (found == null)
            {
                throw new Exception("User not found");
            }
            found = _mapper.Map(newUser, found);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUser(Guid id)
        {
            var found = await _context.Users.FirstOrDefaultAsync(entity => entity.Id == id);
            if (found == null)
            {
                throw new Exception("User not found");
            }
            _context.Users.Remove(found);
            await _context.SaveChangesAsync();
        }
    }
}
