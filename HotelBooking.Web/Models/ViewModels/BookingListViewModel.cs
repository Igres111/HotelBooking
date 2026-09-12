using HotelBooking.Web.Models.Enums;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Models.ViewModels
{
    public class BookingListViewModel
    {
        public PagedResult<BookingResponse>? Bookings { get; set; }
        public PagedResult<BookingResponse>? RecurringBookings { get; set; }
        public BookingStatus? Status { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
