using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class BillingInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public bool PrePaid { get; set; }
        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
