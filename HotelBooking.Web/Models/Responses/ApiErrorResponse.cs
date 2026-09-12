namespace HotelBooking.Web.Models.Responses
{
    public record ApiErrorResponse(int StatusCode, string Message, string Details);
}
