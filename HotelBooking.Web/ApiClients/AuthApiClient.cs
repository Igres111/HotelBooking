using System.Net.Http.Json;
using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.ApiClients
{
    public class AuthApiClient(HttpClient httpClient) : IAuthApiClient
    {
        public async Task<ResponseWrapper<int>> SignUp(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/signup", request, cancellationToken);

            return await ApiResponseHelper.ParseResponse<int>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<UserResponse>> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            return await ApiResponseHelper.ParseResponse<UserResponse>(response, cancellationToken);
        }
    }
}
