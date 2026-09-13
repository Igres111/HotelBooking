namespace HotelBooking.Web.Models.Responses
{
    public record DashboardResponse(
        int TotalBookings,
        int PendingBookings,
        int ConfirmedBookings,
        int CancelledBookings);
}
