using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.ApiClients.Interfaces
{
    public interface IAuthApiClient
    {
        Task<ResponseWrapper<int>> SignUp(RegisterUserRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<UserResponse>> Login(LoginUserRequest request, CancellationToken cancellationToken);
    }
}
