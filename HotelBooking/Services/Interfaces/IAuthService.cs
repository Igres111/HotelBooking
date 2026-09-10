using HotelBooking.Models.Entities;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseWrapper<int>> Register(RegisterUserRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<User>> Login(LoginUserRequest request, CancellationToken cancellationToken);
    }
}
