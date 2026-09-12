using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Models.ViewModels
{
    public class RoomCatalogViewModel
    {
        public PagedResult<MeetingRoomsResponse>? Rooms { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public int? MinCapacity { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
