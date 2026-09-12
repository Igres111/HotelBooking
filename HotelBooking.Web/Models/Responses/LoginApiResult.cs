namespace HotelBooking.Web.Models.Responses
{
    public record LoginApiResult(ResponseWrapper<UserResponse> Response, string? ApiAuthCookie);
}
