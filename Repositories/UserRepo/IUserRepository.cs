using HotelBooking.DTOs.UserDtos;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Repositories.UserRepo
{
    public interface IUserRepository
    {
        public Task<List<ReceiveUserDto>> ReceiveUsers();
        public Task RegisterUser(RegisterDto user);
        public Task<ReceiveTokenDto> LoginUser(LoginDto user);
        public Task UpdateUser(Guid id, UpdateUserDto newUser);
        public Task DeleteUser(Guid id);
    }
}
