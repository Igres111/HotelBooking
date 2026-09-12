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

        public async Task<LoginApiResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            var parsedResponse = await ApiResponseHelper.ParseResponse<UserResponse>(response, cancellationToken);

            string? apiAuthCookie = null;

            if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
            {
                var authCookieHeader = setCookieHeaders.FirstOrDefault(header => header.StartsWith("HotelBooking.Auth="));

                if (authCookieHeader is not null)
                {
                    apiAuthCookie = authCookieHeader.Split(';')[0];
                }
            }

            return new LoginApiResult(parsedResponse, apiAuthCookie);
        }

        public async Task<ResponseWrapper> Logout(CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync("api/auth/logout", null, cancellationToken);

            return await ApiResponseHelper.ParseResponse<object>(response, cancellationToken);
        }
    }
}
