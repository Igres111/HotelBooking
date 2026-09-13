using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelBooking.Web.ApiClients
{
    public class DashboardApiClient(HttpClient httpClient) : IDashboardApiClient
    {
        public async Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string?>();

            if (request.Year.HasValue)
            {
                queryParams["Year"] = request.Year.Value.ToString();
            }

            if (request.Month.HasValue)
            {
                queryParams["Month"] = request.Month.Value.ToString();
            }

            var url = QueryHelpers.AddQueryString("api/dashboard", queryParams);

            var response = await httpClient.GetAsync(url, cancellationToken);

            return await ApiResponseHelper.ParseResponse<DashboardResponse>(response, cancellationToken);
        }
    }
}
