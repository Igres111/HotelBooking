using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Models.ViewModels
{
    public class RoomAvailabilityViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int DurationMinutes { get; set; }
        public string TimeZoneId { get; set; } = string.Empty;
        public List<TimeSlotResponse>? TimeSlots { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
