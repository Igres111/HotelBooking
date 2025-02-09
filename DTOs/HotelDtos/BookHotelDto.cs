namespace HotelBooking.DTOs.HotelDtos
{
    public class BookHotelDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public bool PrePaid { get; set; }
    }
}
