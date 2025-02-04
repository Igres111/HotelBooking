using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs.ReviewDtos
{
    public class RegisterReviewDto
    {
        [Required]
        public Guid HotelId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public float Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
