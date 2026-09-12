namespace HotelBooking.Web.Models.Responses
{
    public record ResponseWrapper(bool IsSuccess, int StatusCode, string Message);

    public record ResponseWrapper<T>(bool IsSuccess, int StatusCode, string Message, T? Data)
        : ResponseWrapper(IsSuccess, StatusCode, Message);
}
