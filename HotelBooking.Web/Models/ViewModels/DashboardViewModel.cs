using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }

        public List<MeetingRoomsResponse> Rooms { get; set; } = [];
        public int? RoomId { get; set; }
        public DateOnly UtilizationDate { get; set; }
        public TimeOnly FromTime { get; set; }
        public TimeOnly ToTime { get; set; }
        public List<TimeSlotResponse> OccupiedSlots { get; set; } = [];
    }
}
