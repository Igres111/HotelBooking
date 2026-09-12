using System.Text.Json;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Helpers
{
    public static class ApiResponseHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<ResponseWrapper<T>> ParseResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
            {
                return new ResponseWrapper<T>(false, (int)response.StatusCode, response.ReasonPhrase ?? "An error occurred.", default);
            }

            using var document = JsonDocument.Parse(content);

            if (document.RootElement.TryGetProperty("isSuccess", out _))
            {
                return JsonSerializer.Deserialize<ResponseWrapper<T>>(content, JsonOptions)!;
            }

            var error = JsonSerializer.Deserialize<ApiErrorResponse>(content, JsonOptions)!;

            return new ResponseWrapper<T>(false, error.StatusCode, error.Message, default);
        }
    }
}
