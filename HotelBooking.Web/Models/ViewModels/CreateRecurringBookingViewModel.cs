namespace HotelBooking.Web.Models.ViewModels
{
    public class CreateRecurringBookingViewModel
    {
        public int RoomId { get; set; }
        public DateOnly StartDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string TimeZoneId { get; set; } = string.Empty;
        public int AttendeeCount { get; set; }
        public string? Notes { get; set; }
        public int OccurrenceCount { get; set; }
    }
}
