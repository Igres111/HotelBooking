using System.ComponentModel.DataAnnotations;
using HotelBooking.Models.BaseTypes;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Entities
{
    public class Booking : BaseEntity
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public MeetingRoom? Room { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public int AttendeeCount { get; set; }
        public string? Notes { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int? RecurringSeriesId { get; set; }
        public RecurringBookingSeries? RecurringSeries { get; set; }

        public string? IdempotencyKey { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public List<BookingStatusHistory> StatusHistory { get; set; } = [];
    }
}
