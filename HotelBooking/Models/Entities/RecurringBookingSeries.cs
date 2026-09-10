using HotelBooking.Models.BaseTypes;

namespace HotelBooking.Models.Entities
{
    public class RecurringBookingSeries : BaseEntity
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public MeetingRoom? Room { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int OccurrenceCount { get; set; }

        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
