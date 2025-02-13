using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs.BookingDtos
{
    public class BookingInfoDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid HotelId { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be at least 1.")]
        public int NumberOfGuests { get; set; }
        [Required]
        public bool PrePaid { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive value.")]
        public decimal TotalPrice { get; set; }
    }
}
