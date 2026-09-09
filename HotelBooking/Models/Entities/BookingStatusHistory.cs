using HotelBooking.Models.BaseTypes;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Entities
{
    public class BookingStatusHistory : BaseEntity
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public BookingStatus PreviousStatus { get; set; }
        public BookingStatus NewStatus { get; set; }

        public int ActingUserId { get; set; }
        public User? ActingUser { get; set; }
        public string? Reason { get; set; }
    }
}
