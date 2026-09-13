using System.Net.Http.Json;
using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelBooking.Web.ApiClients
{
    public class BookingApiClient(HttpClient httpClient) : IBookingApiClient
    {
        public async Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(GetBookingsRequest request, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = request.Page.ToString(),
                ["PageSize"] = request.PageSize.ToString(),
                ["SortDescending"] = request.SortDescending.ToString()
            };

            if (request.Status.HasValue)
            {
                queryParams["Status"] = request.Status.Value.ToString();
            }

            if (request.IsRecurring.HasValue)
            {
                queryParams["IsRecurring"] = request.IsRecurring.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                queryParams["SortBy"] = request.SortBy;
            }

            var url = QueryHelpers.AddQueryString("api/booking", queryParams);

            var response = await httpClient.GetAsync(url, cancellationToken);

            return await ApiResponseHelper.ParseResponse<PagedResult<BookingResponse>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("api/booking/admin", cancellationToken);

            return await ApiResponseHelper.ParseResponse<List<BookingResponse>>(response, cancellationToken);
        }

        public async Task<byte[]> ExportFile(CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("api/booking/admin/export", cancellationToken);

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"api/booking/{id}", cancellationToken);

            return await ApiResponseHelper.ParseResponse<BookingResponse>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<int>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/booking")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            return await ApiResponseHelper.ParseResponse<int>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<List<int>>> CreateRecurring(CreateRecurringBookingRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/booking/recurring")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            return await ApiResponseHelper.ParseResponse<List<int>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> Cancel(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync($"api/booking/{id}/cancel", null, cancellationToken);

            return await ApiResponseHelper.ParseResponse<BookingResponse>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> Confirm(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync($"api/booking/{id}/confirm", null, cancellationToken);

            return await ApiResponseHelper.ParseResponse<BookingResponse>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> Reject(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync($"api/booking/{id}/reject", null, cancellationToken);

            return await ApiResponseHelper.ParseResponse<BookingResponse>(response, cancellationToken);
        }
    }
}
