namespace HotelBooking.Services.Interfaces
{
    public interface ITimeZoneConverter
    {
        DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId);
        DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId);
    }
}
