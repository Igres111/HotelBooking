using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelBooking.Services
{
    public class TokenGenerator
    {
        public readonly Context _context;
        public readonly IConfiguration _configuration;
        public TokenGenerator(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public string CreateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = Environment.GetEnvironmentVariable("Key");

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("JWT secret key is not set in the environment variables.");
            }
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.FullName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.Now.AddDays(7),
                UserId = user.Id,
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
        public async Task<string> RefreshAccessTokenAsync(string refreshTokenInput)
        {
            var refreshToken = await _context.RefreshTokens.Include(el => el.User)
                .Include(el => el.User)
                .FirstOrDefaultAsync(el => el.Token == refreshTokenInput);
            if (refreshToken == null || refreshToken.ExpirationDate < DateTime.Now)
            {
                throw new SecurityTokenException("Refresh token expired or invalid");
            }
            var newAccessToken = CreateAccessToken(refreshToken.User);
            var newRefreshToken = await CreateRefreshTokenAsync(refreshToken.User);

            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
            return newAccessToken;
        }
    }
}

