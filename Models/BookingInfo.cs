using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class BookingInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public bool PrePaid { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
