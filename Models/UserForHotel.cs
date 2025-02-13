namespace HotelBooking.Models
{
    public class UserForHotel
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Hotel Hotel { get; set; }
        public Guid HotelId { get; set; }
    }
}
