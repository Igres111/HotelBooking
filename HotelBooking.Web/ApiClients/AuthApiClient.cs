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
            var (antiforgeryCookie, antiforgeryToken) = await FetchAntiforgeryPair(cancellationToken);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/signup")
            {
                Content = JsonContent.Create(request)
            };
            AttachAntiforgeryHeaders(httpRequest, antiforgeryCookie, antiforgeryToken);

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            return await ApiResponseHelper.ParseResponse<int>(response, cancellationToken);
        }

        public async Task<LoginApiResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var (preLoginCookie, preLoginToken) = await FetchAntiforgeryPair(cancellationToken);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/login")
            {
                Content = JsonContent.Create(request)
            };
            AttachAntiforgeryHeaders(httpRequest, preLoginCookie, preLoginToken);

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            var parsedResponse = await ApiResponseHelper.ParseResponse<UserResponse>(response, cancellationToken);

            if (!parsedResponse.IsSuccess)
            {
                return new LoginApiResult(parsedResponse, null, null, null);
            }

            var apiAuthCookie = ExtractSetCookie(response, "HotelBooking.Auth=");

            // The pair fetched above is bound to the anonymous identity that requested it, and won't
            // validate once the caller is authenticated. Fetch a fresh pair as the now-authenticated
            // user for reuse on every subsequent authenticated call.
            var (antiforgeryCookie, antiforgeryToken) = await FetchAntiforgeryPair(cancellationToken, apiAuthCookie);

            return new LoginApiResult(parsedResponse, apiAuthCookie, antiforgeryCookie, antiforgeryToken);
        }

        public async Task<ResponseWrapper> Logout(CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync("api/auth/logout", null, cancellationToken);

            return await ApiResponseHelper.ParseResponse<object>(response, cancellationToken);
        }

        private async Task<(string? Cookie, string? Token)> FetchAntiforgeryPair(CancellationToken cancellationToken, string? apiAuthCookie = null)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/antiforgery/token");

            if (!string.IsNullOrEmpty(apiAuthCookie))
            {
                httpRequest.Headers.Add("Cookie", apiAuthCookie);
            }

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            var cookie = ExtractSetCookie(response, "HotelBooking.Antiforgery=");
            var body = await response.Content.ReadFromJsonAsync<AntiforgeryTokenResponse>(cancellationToken: cancellationToken);

            return (cookie, body?.Token);
        }

        private static void AttachAntiforgeryHeaders(HttpRequestMessage request, string? antiforgeryCookie, string? antiforgeryToken)
        {
            if (!string.IsNullOrEmpty(antiforgeryCookie))
            {
                request.Headers.Add("Cookie", antiforgeryCookie);
            }

            if (!string.IsNullOrEmpty(antiforgeryToken))
            {
                request.Headers.Add("X-CSRF-TOKEN", antiforgeryToken);
            }
        }

        private static string? ExtractSetCookie(HttpResponseMessage response, string cookiePrefix)
        {
            if (!response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
            {
                return null;
            }

            var cookieHeader = setCookieHeaders.FirstOrDefault(header => header.StartsWith(cookiePrefix));

            return cookieHeader?.Split(';')[0];
        }
    }
}
