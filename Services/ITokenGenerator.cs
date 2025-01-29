using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface ITokenGenerator
    {
        public string CreateAccessToken(User user);
        public Task<RefreshToken> CreateRefreshTokenAsync(User user);
        public Task<string> RefreshAccessTokenAsync(string refreshTokenInput);
    }
}
