namespace HotelBooking.Web.Models.ViewModels
{
    public class CreateMeetingRoomViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
    }
}
