namespace HotelBooking.Models
{
    public class UserForHotel
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Hotel Hotel { get; set; }
        public Guid HotelId { get; set; }
    }
}
