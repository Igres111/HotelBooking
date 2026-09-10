using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseWrapper<int>> Register(RegisterUserRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<UserResponse>> Login(LoginUserRequest request, CancellationToken cancellationToken);
    }
}
