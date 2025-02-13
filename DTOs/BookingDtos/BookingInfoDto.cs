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
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckIn < DateTime.UtcNow.Date)
            {
                yield return new ValidationResult("Check-in date cannot be in the past.", new[] { nameof(CheckIn) });
            }
        }
        [Required]
        public DateTime CheckOut { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        [Required]
        public bool PrePaid { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
    }
}
