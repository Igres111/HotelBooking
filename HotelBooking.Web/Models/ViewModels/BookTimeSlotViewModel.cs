namespace HotelBooking.Web.Models.ViewModels
{
    public class BookTimeSlotViewModel
    {
        public int RoomId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string TimeZoneId { get; set; } = string.Empty;
    }
}
