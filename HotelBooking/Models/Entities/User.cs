using HotelBooking.Models.BaseTypes;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<RecurringBookingSeries> RecurringBookingSeries { get; set; } = new List<RecurringBookingSeries>();
    }
}
