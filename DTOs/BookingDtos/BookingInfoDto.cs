namespace HotelBooking.DTOs.BookingDtos
{
    public class BookingInfoDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public bool PrePaid { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
